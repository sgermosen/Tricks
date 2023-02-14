using MersyRd.Services;

namespace MersyRd.ViewModels
{
    public class StudentDashboardViewModel
    {
        public ObservableCollection<UserListResponse> Users { get; set; } =
            new ObservableCollection<UserListResponse>();

        private readonly ILoginService _loginService;
        public StudentDashboardViewModel(ILoginService loginService)
        {
            _loginService = loginService;
            GetAllUsers();
        }

        private void GetAllUsers()
        {
            Task.Run(async () =>
            {
                var userList = await _loginService.GetAllUsers();

                App.Current.Dispatcher.Dispatch(() =>
                {
                    if (userList?.Count > 0)
                    {
                        foreach (var user in userList)
                        {
                            Users.Add(user);
                        }
                    }
                });
            });
        }
    }
}
