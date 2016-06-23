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
		private static bool FreezeMap = false;
		private static BusMap map;
		private static Xamarin.Forms.Maps.Position currLocation;
		private static double currRadius = 0.5;

	    public MapPage() {
	    	// map with default centre at NUS
			currLocation = new Xamarin.Forms.Maps.Position (1.2966, 103.7764);
	        map = new BusMap(
	            MapSpan.FromCenterAndRadius(currLocation, Distance.FromKilometers(SettingsVars.MEAN_MAP_RADIUS))) {
	                IsShowingUser = true,
	                HeightRequest = 100,
	                WidthRequest = 960,
	                VerticalOptions = LayoutOptions.FillAndExpand
	            };

	        // shift to current location if possible (activate only for device testing)
			// ShiftToCurrentLocation ();

	        // slider to change radius from 0.1 - 0.9 km (for simulator testing)
			var slider = new Slider (1, 9, 5);
			slider.ValueChanged += (sender, e) => {
			    var zoomLevel = e.NewValue; // between 1 and 9
			    currRadius = (SettingsVars.MEAN_MAP_RADIUS * 2) - (zoomLevel/(SettingsVars.MEAN_MAP_RADIUS * 20));
			    map.MoveToRegion(MapSpan.FromCenterAndRadius(
			    	map.VisibleRegion.Center, Distance.FromKilometers(currRadius)));
			};

			// add map and slider to stack layout
	        var stack = new StackLayout { Spacing = 0 };
	        stack.Children.Add(map);
			stack.Children.Add(slider);

			Icon = "MapTabIcon.png";
			Title = "Map";
	        Content = stack;

	        // add random buses for testing
			BusSimulator.DispatchBuses ();

			// init bus and stop pins
			UpdateBusPins ();
			UpdateStopPins ();

	        // set timer to update bus, stops at interval
			Device.StartTimer (TimeSpan.FromSeconds(SettingsVars.REFRESH_BUS_INTERVAL), UpdateBusPins);
			Device.StartTimer (TimeSpan.FromSeconds(SettingsVars.REFRESH_STOP_INTERVAL), UpdateStopPins);
	    }

	    public static void CentraliseMap (Xamarin.Forms.Maps.Position pos) {
	    	map.MoveToRegion(MapSpan.FromCenterAndRadius(
				pos, Distance.FromKilometers (currRadius)));
	    }

	    public static void SetFreezeMap (bool value) {
			FreezeMap = value;
	    }

		private void ShiftToCurrentLocation () {
			GetCurrentPosition ().ContinueWith(t => {
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
	                currLocation = new Xamarin.Forms.Maps.Position (t.Result.Latitude, t.Result.Longitude);

	                map.MoveToRegion(MapSpan.FromCenterAndRadius(currLocation, Distance.FromKilometers(SettingsVars.MEAN_MAP_RADIUS)));
	            }
	        }, TaskScheduler.FromCurrentSynchronizationContext ());
	    }

		private async Task<XLabs.Platform.Services.Geolocation.Position> GetCurrentPosition() {
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

	    private bool UpdateBusPins() {
			// skip update pins if map is freezed
			if (FreezeMap)
				return true;

			// remove all bus pins
			foreach (CustomPin p in map.BusPins)
				map.Pins.Remove (p.Pin);
			map.BusPins.Clear ();

			foreach (BusOnRoad bor in BusHelper.ActiveBuses.Values) {
				// move bus to next checkpoint on the route for simulation
				BusSimulator.GoToNextCheckpoint (bor);

				// add pin to map if svc show on map
				if (BusHelper.BusSvcs [bor.routeName].showOnMap) {
					var description = "Start: " + BusHelper.BusStops [bor.firstStop].name + "\n" +
					                  "End: " + BusHelper.BusStops [bor.lastStop].name + "\n" +
					                  "Approaching: " + BusHelper.BusStops [(string)bor.nextStopEnumerator.Current].name + "\n" +
					                  "In: " + BusHelper.GetArrivalTiming ((string)bor.nextStopEnumerator.Current, bor.routeName) + "\n";
					var pin = new Pin {
						Type = PinType.Place,
						Position = new Xamarin.Forms.Maps.Position (bor.latitude, bor.longitude),
						Label = bor.routeName,
						Address = description
					};
					var bus = new CustomPin {
						Pin = pin,
						Id = bor.routeName,
						Url = bor.routeName + ".png"
					};
					map.Pins.Add (pin);
					map.BusPins.Add (bus);
				}
			}

			return true;
	    }

	    private bool UpdateStopPins() {
			// skip update pins if map is freezed
			if (FreezeMap)
				return true;

			// remove all stop pins
			foreach (CustomPin p in map.StopPins)
				map.Pins.Remove (p.Pin);
			map.StopPins.Clear ();

			// add stop pins, with change in arrivatl timing
	        foreach (BusStop busStop in BusHelper.BusStops.Values) {
	        	var description = "";
	        	foreach (string svc in busStop.services) 
					description += svc + ": " + BusHelper.GetArrivalTiming (busStop.busStopCode, svc) + "\n";
				var pin = new Pin {
		            Type = PinType.Place,
					Position = new Xamarin.Forms.Maps.Position(busStop.latitude, busStop.longitude),
		            Label = busStop.name + " - " + busStop.busStopCode,
					Address = description
		        };
		        var stop = new CustomPin {
					Pin = pin,
					Id = "stop",
					Url = "stop.png"
				};
				map.Pins.Add(pin);
				map.StopPins.Add (stop);
			}

			return true;
	    }
	}
}

