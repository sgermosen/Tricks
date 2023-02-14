namespace sysmed.Views;

public partial class PatientAddUpdatePage : ContentPage
{
	public PatientAddUpdatePage(PatientAddUpdateViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}