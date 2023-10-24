using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;

namespace MauiGeolocation;

public partial class MainPage : ContentPage
{
	private CancellationTokenSource cancelTokenSource;
	private bool currentlyCheckingLocation = false;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void GetCurrentLocation(Object sender, EventArgs e)
	{
		await GetCurrentLocationTask();
	}

	private async Task GetCurrentLocationTask()
	{
		try
		{
			currentlyCheckingLocation = true;
			cancelTokenSource = new CancellationTokenSource();

			GeolocationRequest geoRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
			Location location = await Geolocation.Default.GetLocationAsync(geoRequest, cancelTokenSource.Token);

			if(location != null)
			{
				Debug.WriteLine($"\n{location}");
			}
		}
		catch(PermissionException permissionException)
		{
			Debug.WriteLine($"Permission Exception {permissionException.Message}");
		}
		catch(FeatureNotEnabledException featureNotEnabledException)
		{
            Debug.WriteLine($"Feature Enabled Exception {featureNotEnabledException.Message}");
        }
		catch(FeatureNotSupportedException featureNotSupportedException)
		{
            Debug.WriteLine($"Feature Supported Exception {featureNotSupportedException.Message}");
        }
		catch(Exception ex)
		{
            Debug.WriteLine($"Exception {ex.Message}");
        }
		finally
		{

		}
	}
}


