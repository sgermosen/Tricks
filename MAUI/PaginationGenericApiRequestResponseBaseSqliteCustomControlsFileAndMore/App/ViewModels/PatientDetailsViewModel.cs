using sysmed.Services;
using sysmed.Views;

namespace sysmed.ViewModel;

[QueryProperty("Patient", "Patient")]
public partial class PatientDetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    Patient patient = new();

    private readonly IPatientService _PatientService;
    public PatientDetailsViewModel(IPatientService PatientService)
    {
        _PatientService = PatientService;
    }
     
    [RelayCommand]
    async Task GoToImages()
    { 
       //
    }
    [RelayCommand]
    async Task GoToAppointments()
    {
       //
    }

}
