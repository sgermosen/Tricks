
using sysmed.ViewModels;

namespace sysmed.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}