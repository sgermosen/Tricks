using sysmed.Views;
using Newtonsoft.Json;

namespace sysmed.ViewModels
{
    public class LoadingViewModel
    {
        public LoadingViewModel()
        { 
            CheckUserLoginDetails();
        }
        private async void CheckUserLoginDetails()
        {
            await Task.Delay(3000);
            string userDetailsStr = Preferences.Get(nameof(App.UserDetails), "");

            if (string.IsNullOrWhiteSpace(userDetailsStr))
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                // navigate to Login Page
            }
            else
            {
                var tokenDetails = await SecureStorage.GetAsync(nameof(App.Token));

                if (string.IsNullOrEmpty(tokenDetails))//jsonToken.ValidTo < DateTime.UtcNow)
                {
                    await Shell.Current.DisplayAlert("User session expired", "Login Again To conitnue", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                else
                {
                    var userInfo = JsonConvert.DeserializeObject<UserBasicInfo>(userDetailsStr);
                    App.UserDetails = userInfo;
                    App.Token = tokenDetails;
                    await Shell.Current.GoToAsync($"//{nameof(PatientsPage)}");
                }


            }
        }

    }
}
