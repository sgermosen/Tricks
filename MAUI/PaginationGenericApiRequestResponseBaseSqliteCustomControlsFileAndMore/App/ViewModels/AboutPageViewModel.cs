using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace sysmed.ViewModels
{
    public class AboutPageViewModel : BaseViewModel
    {
        public ICommand TapCommand { get; set; }
        public ICommand EmailCommand { get; set; }
        //[RelayCommand]
        //public async void Tab(string url)
        //{
        //    await Launcher.OpenAsync(url);
        //}
        public AboutPageViewModel()
        {
            TapCommand = new Command<string>(OpenBrowser);
            EmailCommand = new Command<string>(SendEmail);
        }
        private async void SendEmail(string email)
        {
            var message = new EmailMessage
            {
                To = new List<string> { email },
                Subject = "I want to hire you",
            };

            await Email.ComposeAsync(message);
        }
        private async void OpenBrowser(string url)
        {
            await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
    }

}
