namespace MersyRd.Views;

//[QueryProperty("Monkey", "Monkey")]
public partial class DetailsPage : ContentPage
{
	//	public Monkey Monkey { get; set; } //this will automatically be binded with the query property
	//but is not neccesary because MAUI knous things and automatically map thins for you, 
	//So  we move this to viewmodel
	public DetailsPage(MonkeyDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	//here is where we are suposed to make the convertion
	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{ 
		base.OnNavigatedTo(args);
	}
}