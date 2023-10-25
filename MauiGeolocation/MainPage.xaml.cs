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
		await CheckAndRequestLocationPermission();
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

    public async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            return status;
        }

        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }

        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status;
    }
}


