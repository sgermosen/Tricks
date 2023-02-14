using sysmed.Controls;
using sysmed.Services;
using sysmed.Views;
using System.Windows.Input;

namespace sysmed.ViewModel;

public partial class PatientsViewModel : BaseViewModel
{
    public ObservableCollection<Patient> Patients { get; set; } = new();

    readonly IConnectivity connectivity;
    private readonly IPatientService _PatientService;
    int pageNumber = 1;
    readonly int pageSize = 10; [ObservableProperty]
    string searchTerm = string.Empty;
    [ObservableProperty]
    bool hasMoreItems = true; 

    public Command LoadMoreItemsCommand { get; set; }

    public PatientsViewModel(IConnectivity connectivity, IPatientService PatientService)
    {
        AppShell.Current.FlyoutHeader = new FlyoutHeaderControl();
        this.connectivity = connectivity;
        _PatientService = PatientService;
        Title = "Listado de Pacientes"; 
        LoadMoreItemsCommand = new Command(LoadItems);

    }

    ICommand loadMoreCommand = null;
    public ICommand LoadMoreCommand
    {
        get { return loadMoreCommand; }
        set
        {
            if (loadMoreCommand != value)
            {
                loadMoreCommand = value;
                OnPropertyChanged(nameof(LoadMoreCommand));
            }
        }
    }

    [RelayCommand]
    async Task GoToDetails(Patient Patient)
    {
        if (Patient is null)
            return;
        await Shell.Current.GoToAsync($"{nameof(PatientDetailsPage)}", true,
            new Dictionary<string, object>
            {
                {"Patient", Patient }
            });
    }

    [RelayCommand]
    void GetPatientsSearch()
    {
        hasMoreItems = true;
        pageNumber = 1;
        if (Patients.Count != 0)
            Patients.Clear();
        LoadItems(); 
       
    }

    [RelayCommand]
    public void GetMoreItems()
    {
        LoadItems();
    }

    [RelayCommand]
    public async void AddUpdatePatient()
    {
        await AppShell.Current.GoToAsync(nameof(PatientAddUpdatePage));
    }

    [RelayCommand]
    public async void DisplayAction(Patient model)
    {
        var response = await AppShell.Current.DisplayActionSheet("Select Option", "OK", null, "Edit", "Delete");
        if (response == "Edit")
        {
            var navParam = new Dictionary<string, object>
            {
                { "Patient", model }
            };
            await AppShell.Current.GoToAsync(nameof(PatientDetailsPage), navParam);
        }
        else if (response == "Delete")
        {
            var delResponse = await _PatientService.Delete(model);
            if (delResponse > 0)
            {
                GetPatientsSearch();
            }
        }
    }

    private async void LoadItems()
    {
        
        if (IsBusy)
            return;
        if (!HasMoreItems)
        {
            IsBusy = false;
            IsRefreshing = false;
            return;
        }
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet issue", $"Check your internet and try again", "Ok");
                return;
            }
            IsBusy = true;
            IsRefreshing = true;
            var request = new Request
            {
                SearchTerm = SearchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _PatientService.Get<List<Patient>>(request);
            if (!response.IsSuccess)
                await Shell.Current.DisplayAlert("Error!", $"No fue posible obtener los pacientes: {response.Message}", "Ok");

            foreach (var patient in response.Data)
                Patients.Add(patient);

            if (Patients.Count != response.ItemsCount)
            {
                HasMoreItems = true;
                pageNumber++;
            }
            else
            {
                HasMoreItems = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!", $"No fue posible obtener los pacientes: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
