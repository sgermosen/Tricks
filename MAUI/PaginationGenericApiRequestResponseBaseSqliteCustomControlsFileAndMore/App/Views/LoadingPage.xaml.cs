using sysmed.ViewModels;

namespace sysmed.Views;

public partial class LoadingPage : ContentPage
{
    public LoadingPage(LoadingViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}