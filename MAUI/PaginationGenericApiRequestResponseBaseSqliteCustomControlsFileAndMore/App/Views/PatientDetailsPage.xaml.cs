namespace sysmed.Views;
 
public partial class PatientDetailsPage : ContentPage
{
	public PatientDetailsPage(PatientDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
	}
}