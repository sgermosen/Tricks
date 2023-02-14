namespace MersyRd.ViewModel;

[QueryProperty("Monkey", "Monkey")]
public partial class MonkeyDetailsViewModel : BaseViewModel
{
	IMap map;
	public MonkeyDetailsViewModel(IMap map)
	{
		this.map = map;
	}

	[ObservableProperty]
	Monkey monkey;

	[RelayCommand]
	async Task OpenMapAsync()
	{
		try
		{
			await map.OpenAsync(Monkey.Latitude, Monkey.Longitude,
				new MapLaunchOptions
				{
					Name = Monkey.Name,
					NavigationMode = NavigationMode.None
				});

		}
		catch (Exception ex)
		{

			Debug.WriteLine(ex);
			await Shell.Current.DisplayAlert("Error!",
				$"Unable to open Map: {ex.Message}", "Ok");

		}
	}
	//[RelayCommand]
	//async Task GoBackAsync()
	//{
	//	await Shell.Current.GoToAsync("..?id=1"); //passing parameters
	//       await Shell.Current.GoToAsync("../.."); //goBack and then go back again
	//	//await Shell.Current.GoToAsync("../DetailsPage");//go back and go to details page
	//   }
}
