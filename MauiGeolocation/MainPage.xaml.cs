using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MauiGeolocation;

public partial class MainPage : ContentPage
{
	private bool setupComplete = false;
	private Location lastLocation;
    private Map map;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void GetCurrentLocation(Object sender, EventArgs e)
	{
		if(!setupComplete)
		{
            await CheckAndRequestLocationPermission();
            await GetCurrentLocationTask();
            CreateMap();
        }
		else
		{
            await GetCurrentLocationTask();

            MapSpan mapSpan = new MapSpan(lastLocation, 0.01, 0.01);
            map.MoveToRegion(mapSpan);

            Pin newPin = new Pin
            {
                Label = "location",
                Location = lastLocation,
                Type = PinType.Generic
            };

            map.Pins.Add(newPin);
        }
    }

    private async Task<PermissionStatus> CheckAndRequestLocationPermission()
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

    private async Task GetCurrentLocationTask()
	{
		try
		{
			CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

			GeolocationRequest geoRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
			Location location = await Geolocation.Default.GetLocationAsync(geoRequest, cancelTokenSource.Token);

			if(location != null)
			{
				Debug.WriteLine($"\n{location}");
				lastLocation = location;
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
	}

	private void CreateMap()
	{
	    map = new Map
		{
			HeightRequest = 1000,
			WidthRequest = 1000,
			MapType = MapType.Satellite
		};

        holder.Add(map);


		MapSpan mapSpan = new MapSpan(lastLocation, 0.01, 0.01);
        map.MoveToRegion(mapSpan);

        Pin newPin = new Pin
        {
            Label = "location",
            Location = lastLocation,
            Type = PinType.Generic
        };

        map.Pins.Add(newPin);

        setupComplete = true;
    }
}


