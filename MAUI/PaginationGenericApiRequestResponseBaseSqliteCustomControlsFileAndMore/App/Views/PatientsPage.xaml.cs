namespace sysmed.Views;

public partial class PatientsPage : ContentPage
{
    private readonly PatientsViewModel _viewModel;
    public PatientsPage(PatientsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel; 
        BindingContext = viewModel;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetPatientsSearchCommand.Execute(null);
    }
}

