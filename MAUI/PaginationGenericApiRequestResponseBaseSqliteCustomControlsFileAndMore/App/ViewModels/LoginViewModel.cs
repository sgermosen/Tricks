using Newtonsoft.Json;
using sysmed.Services;
using sysmed.Views;

namespace sysmed.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        private readonly ApiService _apiService;

        public LoginViewModel()
        {
            _apiService = new ApiService();
            Email = "mersyrd@gmail.com";
            Password = "824455";
        }

        #region Commands
        [RelayCommand]
        async void Login()
        {
            IsBusy = true;
            if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                LoginModel userMoel = new LoginModel()
                {
                    Email = Email,
                    Password = Password,
                };
                LoginResponseModel objResponse = await BaseViewModel.MyLoginService.PerformLogin(userMoel);
                if (objResponse == null || string.IsNullOrEmpty(objResponse.token))
                {
                    IsBusy = false;
                    await AppShell.Current.DisplayAlert("Error de Acceso", "Usuario o Contraseña incorrectos", "OK");
                    Password = null;
                    return;
                } 

                var user = new UserBasicInfo();
                user.FirstName = objResponse.displayName;
                user.Email = objResponse.username;
                user.Picture = objResponse.image;
                user.UserTypeId = 1;

                user.AccessToken = objResponse.token;
                user.TokenType = "Bearer";// token.TokenType;
                user.TokenExpires = new DateTime(2025, 10, 10);// token.Expires;
                user.IsRemembered = true;
                user.Password = Password;

                if (Preferences.ContainsKey(nameof(App.UserDetails)))
                {
                    Preferences.Remove(nameof(App.UserDetails));
                }

                await SecureStorage.SetAsync(nameof(App.Token), objResponse.token);

                string userDetailStr = JsonConvert.SerializeObject(user);
                Preferences.Set(nameof(App.UserDetails), userDetailStr);
                App.UserDetails = user;
                App.Token = objResponse.token;
                IsBusy = false;
                await Shell.Current.GoToAsync($"//{nameof(PatientsPage)}");
                IsBusy = false;
            }
            else
            {
                IsBusy = true;
                await AppShell.Current.DisplayAlert("Error de Acceso", "Te falta llenar el email o la contraseña", "OK");
            }

        }

        [RelayCommand]
        async void Register()
        {
            await Shell.Current.GoToAsync($"//{nameof(PatientsPage)}");
        }

        [RelayCommand]
        async void ForgotPassword()
        {
            await Shell.Current.GoToAsync($"//{nameof(PatientsPage)}");
        }

        [RelayCommand]
        async void SignUp()
        {
            await Shell.Current.GoToAsync($"//{nameof(PatientsPage)}");
        }
        #endregion
    }
}

