using sysmed.ViewModels;
using sysmed.Views;

namespace sysmed;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        this.BindingContext = new AppShellViewModel(); 
        Routing.RegisterRoute(nameof(PatientDetailsPage), typeof(PatientDetailsPage)); 
        Routing.RegisterRoute(nameof(PatientAddUpdatePage), typeof(PatientAddUpdatePage));  
    }
}

