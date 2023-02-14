using MersyRd.Services;
using MersyRd.Views;

namespace MersyRd.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
	readonly MonkeyService monkeyService;
	public ObservableCollection<Monkey> Monkeys { get; } = new();

	//public Command GetMonkeysCommand { get; }
	readonly IConnectivity connectivity;
	readonly IGeolocation geolocation;
	public MonkeysViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation)
	{
		Title = "Monkye Finder";
		this.monkeyService = monkeyService;
		this.connectivity = connectivity;
		this.geolocation = geolocation;
		//GetMonkeysCommand = new Command(async ()=> await GetMonkeysAsync());// DoSomething); //the toolkit helps better

	}
	//void DoSomething()
	//{
	//invoke async things
	//}

	[ObservableProperty]
	bool isRefreshing;

	[RelayCommand]
	async Task GetClosestMonkeyAsync()//the Rellay generator will remove the word async for naming the command
	{
		if (IsBusy || Monkeys.Count == 0)
			return;

		try
		{
			//Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
			var location = await geolocation.GetLastKnownLocationAsync();
			if (location is null)//or location is too old if you want
			{
				location = await geolocation.GetLocationAsync(
					new GeolocationRequest
					{
						DesiredAccuracy = GeolocationAccuracy.Medium,
						Timeout = TimeSpan.FromSeconds(30)
					});
			}
			if (location is null)
				return;

			var first = Monkeys
				.OrderBy(m => location.
				CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Miles))
				.FirstOrDefault();
			if (first is null)
				return;

			await Shell.Current.DisplayAlert("Closest Monkey",
				$"{first.Name} in {first.Location}", "Ok");

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Shell.Current.DisplayAlert("Error!", $"Unable to get the closest monkey: {ex.Message}", "Ok");

		}
	}

	[RelayCommand]
	async Task GoToDetails(Monkey monkey)
	{
		if (monkey is null)
			return;
		await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
			new Dictionary<string, object>
			{
				{"Monkey", monkey }
			});//?id{monkey.Name}");// ($"details"); //and you will need to register that routes
	}

	[RelayCommand] //it helps to write less code if we have community toolkit
	async Task GetMonkeysAsync()
	{
		//IsRefreshing = false;
		if (IsBusy)
			return;
		try
		{
			if (connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Internet issue", $"Check your internet and try again", "Ok");
				return;
			}
			IsBusy = true;
			var monkeys = await monkeyService.GetMonkeys();

			if (Monkeys.Count != 0)
				Monkeys.Clear();

			foreach (var monkey in monkeys)
				Monkeys.Add(monkey);//this will fire the observable collection event change everytime

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Shell.Current.DisplayAlert("Error!", $"Unable to get monkeys: {ex.Message}", "Ok");
			//App.Current.MainPage
		}
		finally //always if was success or not
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}
}
