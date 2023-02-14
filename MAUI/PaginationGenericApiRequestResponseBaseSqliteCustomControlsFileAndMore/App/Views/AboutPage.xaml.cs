
using sysmed.ViewModels;
using System.Windows.Input;

namespace sysmed.Views;

public partial class AboutPage : ContentPage
{ 
    public AboutPage( )
    {
        InitializeComponent();
        BindingContext = new AboutPageViewModel();
    }
}