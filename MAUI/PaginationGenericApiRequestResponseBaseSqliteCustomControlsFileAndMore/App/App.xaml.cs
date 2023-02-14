using Microsoft.Maui.Platform;
using sysmed.Handlers;
using sysmed.Services;

namespace sysmed;

public partial class App : Application
{
    public static UserBasicInfo UserDetails;
    public static string Token;//="x";
    public App()
    {
        InitializeComponent();
        DependencyService.Register<Toast>();
        DependencyService.Register<LoginService>();
        //Border less entry
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
        {
            if (view is BorderlessEntry)
            {
#if __ANDROID__
                handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
#elif __IOS__
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });

        MainPage = new SplashScreen();
        InitAsync().ConfigureAwait(false);
    }
    private async Task InitAsync()
    {
        await Task.Delay(5000);
        MainPage = new AppShell();
    }
}
