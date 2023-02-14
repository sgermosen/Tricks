using sysmed.Services;

namespace sysmed.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    public static ILoginService<LoginModel> MyLoginService => DependencyService.Get<ILoginService<LoginModel>>();
   
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    bool isRefreshing;

    public bool IsNotBusy => !IsBusy;

}
