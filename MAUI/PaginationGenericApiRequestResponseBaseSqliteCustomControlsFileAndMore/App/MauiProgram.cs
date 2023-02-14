using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using sysmed.Services;
using sysmed.ViewModels;
using sysmed.Views;
namespace sysmed;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        //Singleton //create once and return each time the same 
        //trasient //create another each time than you require a instance
        //services ***view models ****pages

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current); 
        builder.Services.AddSingleton<IPatientService, PatientApiService>();


        //Services
        //  builder.Services.AddSingleton<MonkeyService>();
        builder.Services.AddSingleton<ApiService>();

        //ViewModels  
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoadingViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<AboutPageViewModel>(); 
        builder.Services.AddTransient<PatientDetailsViewModel>();
        builder.Services.AddSingleton<PatientsViewModel>(); 
        builder.Services.AddSingleton<PatientAddUpdateViewModel>();
        //Views
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoadingPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<AboutPage>(); 
        builder.Services.AddTransient<PatientDetailsPage>();
        builder.Services.AddSingleton<PatientsPage>(); 
        builder.Services.AddSingleton<PatientAddUpdatePage>();


        return builder.Build();
    }
}
