using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XLabs.Platform.Device;
using XLabs.Platform;
using XLabs.Ioc;
using XLabs.Platform.Services.Geolocation;

namespace NUSBusMap
{
	public class MapPage : ContentPage {
		private Map map;
		private const double DEFAULT_RADIUS = 0.5;

	    public MapPage() {
	    	// map with default centre at NUS
			var NUSCenter = new Xamarin.Forms.Maps.Position (1.2966, 103.7764);
	        map = new Map(
	            MapSpan.FromCenterAndRadius(NUSCenter, Distance.FromKilometers(DEFAULT_RADIUS))) {
	                IsShowingUser = true,
	                HeightRequest = 100,
	                WidthRequest = 960,
	                VerticalOptions = LayoutOptions.FillAndExpand
	            };

	        // shift to current location if possible (activate only for device testing)
			// ShiftToCurrentLocation ();

	        // add pins for each bus stops
	        foreach (BusStop busStop in BusHelper.BusStops.Values) {
				var pin = new Pin {
				            Type = PinType.Place,
							Position = new Xamarin.Forms.Maps.Position(busStop.latitude, busStop.longitude),
				            Label = busStop.name + " - " + busStop.busStopCode,
							Address = "Bus service - Bus arrival timings"
				        };
				map.Pins.Add(pin);
			}

	        // slider to change radius from 0.1 - 0.9 km
			var slider = new Slider (1, 9, 5);
			slider.ValueChanged += (sender, e) => {
			    var zoomLevel = e.NewValue; // between 1 and 9
			    var radius = 1.0 - (zoomLevel/10.0);
			    map.MoveToRegion(MapSpan.FromCenterAndRadius(
			    	map.VisibleRegion.Center, Distance.FromKilometers(radius)));
			};

			// add map and slider to stack layout
	        var stack = new StackLayout { Spacing = 0 };
	        stack.Children.Add(map);
			stack.Children.Add(slider);

			Icon = "MapTabIcon.png";
			Title = "Map";
	        Content = stack;

	    }

		private void ShiftToCurrentLocation () {
			var geolocation = GetCurrentPosition ().ContinueWith(t => {
	            if (t.IsFaulted)
	            {
	                System.Diagnostics.Debug.WriteLine("Error : {0}", ((GeolocationException)t.Exception.InnerException).Error.ToString());
	            }
	            else if (t.IsCanceled)
	            {
	                System.Diagnostics.Debug.WriteLine("Error : The geolocation has got canceled !");
	            }
	            else
	            {
	                var currentLocation = new Xamarin.Forms.Maps.Position (t.Result.Latitude, t.Result.Longitude);

	                map.MoveToRegion(MapSpan.FromCenterAndRadius(currentLocation, Distance.FromKilometers(DEFAULT_RADIUS)));
	            }
	        }, TaskScheduler.FromCurrentSynchronizationContext ());
	    }

		public async Task<XLabs.Platform.Services.Geolocation.Position> GetCurrentPosition() {
			IGeolocator geolocator = Resolver.Resolve<IGeolocator>();

			XLabs.Platform.Services.Geolocation.Position result = null;

	        if (geolocator.IsGeolocationEnabled) {
	            try {
	                if (!geolocator.IsListening)
	                    geolocator.StartListening(1000, 1000);

	                var task = await geolocator.GetPositionAsync(10000);

	                result = task;

	                System.Diagnostics.Debug.WriteLine("[GetPosition] Lat. : {0} / Long. : {1}", result.Latitude.ToString("N4"), result.Longitude.ToString("N4"));
	            }
	            catch (Exception e) {
	                System.Diagnostics.Debug.WriteLine ("Error : {0}", e);
	            }
	        }

	        return result;
	    }
	}
}

