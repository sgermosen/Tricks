using sysmed.Services;

namespace sysmed.ViewModel;

[QueryProperty("Patient", "Patient")]
public partial class PatientAddUpdateViewModel : BaseViewModel
{
    [ObservableProperty]
    Patient patient = new();

    [ObservableProperty]
    BloodType bloodType = new();

    [ObservableProperty]
    List<BloodType> bloodTypes;

    private readonly IPatientService _patientService;
    public PatientAddUpdateViewModel(IPatientService patientService)
    {
        _patientService = patientService;
        BloodTypes = new List<BloodType> {
        new BloodType{ BloodTypeId= 1, Code = "_D", Name="_Desconocido"},
        new BloodType{ BloodTypeId= 2, Code = "O+", Name="O+"},
        new BloodType{ BloodTypeId= 3, Code = "A+", Name="A+"},
        new BloodType{ BloodTypeId= 4, Code = "B+", Name="B+"},
        new BloodType{ BloodTypeId= 5, Code = "AB+", Name="AB+"},
        new BloodType{ BloodTypeId= 6, Code = "O-", Name="O-"},
        new BloodType{ BloodTypeId= 7, Code = "A-", Name="A-"},
        new BloodType{ BloodTypeId= 8, Code = "B-", Name="B-"},
        new BloodType{ BloodTypeId= 9, Code = "AB-", Name="AB-"}
        };
        if (Patient == null)
            Title = "Registrar Paciente";
        else
        {
            Title = "Actualizar Paciente";
            if (Patient.BloodTypeId != 0)
                BloodType = BloodTypes[Patient.BloodTypeId - 1];
        }

    }

    [ObservableProperty]
    ImageSource imageSource;

    [ObservableProperty]
    FileResult picture;

    [RelayCommand]
    public async Task AddUpdatePatient()
    {
        IsBusy = true;
        Response<Patient> response = new Response<Patient>();
        byte[] imageArray = null;
        if (Picture != null && !string.IsNullOrEmpty(Picture.FullPath))
            imageArray = File.ReadAllBytes(Picture.FullPath);

        Patient.ImageArray = imageArray;
        if (Patient.PersonId > 0) { }
        // response = await _patientService.Update(Patient); 
        else
        {
            if (string.IsNullOrEmpty(Patient.Name))
            {
                await Shell.Current.DisplayAlert("Dame lu?", "El nombre es requerido", "OK");
                IsBusy = false;
                return;
            }
            if (string.IsNullOrEmpty(Patient.LastName))
            {
                await Shell.Current.DisplayAlert("Dame lu?", "El apellido es requerido", "OK");
                IsBusy = false;
                return;
            }
            if (string.IsNullOrEmpty(Patient.Email))
            {
                await Shell.Current.DisplayAlert("Dame lu?", "El email es requerido", "OK");
                IsBusy = false;
                return;
            }
            response = await _patientService.Add<Patient>(new Patient
            {
                Rnc = Patient.Rnc,
                Address = Patient.Address,
                Age = Patient.Age,
                BloodTypeId = BloodType.BloodTypeId == 0 ? 1 : BloodType.BloodTypeId,
                Nss = Patient.Nss,
                InsuranceNumber = Patient.InsuranceNumber,
                Name = Patient.Name,
                BornDate = Patient.BornDate,
                Cel = Patient.Cel,
                Tel = Patient.Tel,
                Email = Patient.Email,
                GenderId = Patient.GenderId,//
                InsuranceId = Patient.InsuranceId,//
                LastName = Patient.LastName,
                Record2 = Patient.Record2,//
                Companion = Patient.Companion,//
                CompanionRnc = Patient.CompanionRnc, //
                ImageArray = imageArray
            });
        }
        IsBusy = false;

        if (response.IsSuccess)
        {
            await Shell.Current.DisplayAlert("Pero bieeeeen!!!!", "Registro Guardado!!!", "OK");
            Patient = new();
            ImageSource = null;
            BloodType = new();
            await Shell.Current.GoToAsync("..");
        }
        else
            await Shell.Current.DisplayAlert("Bueeeeeeeeeno!", $"{response.Message}", "OK");

    }

    [RelayCommand]
    public async Task TakePicture()
    {
        var response = await AppShell.Current.DisplayActionSheet("Seleccione una Opcion", "OK", null, "Tomar Foto", "Selecctionar Foto");
        if (response == "Tomar Foto")
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                Picture = await MediaPicker.Default.CapturePhotoAsync();
            }
        }
        else if (response == "Selecctionar Foto")
            Picture = await MediaPicker.PickPhotoAsync();

        if (Picture != null)
            ImageSource = ImageSource.FromFile(Picture.FullPath);

    }

}
