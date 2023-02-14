using MersyRd.ViewModels;

namespace MersyRd.Views;

public partial class StudentDashboardPage : ContentPage
{
    public StudentDashboardPage(StudentDashboardViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;

    }
}