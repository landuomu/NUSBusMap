using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace NUSBusMap
{
	public class MapPage : ContentPage {
		private static bool FreezeMap = false;
		private static BusMap map;
		private static double MEAN_MAP_RADIUS = 0.5; // in km
		private static double currRadius = 0.5; // in km
		private static double MARGIN_OF_ERROR = 10.0; // in m

	    public MapPage() {
	    	// map with default centre at NUS
			var NUSCentre = new Xamarin.Forms.Maps.Position (1.2966, 103.7764);
	        map = new BusMap(
				MapSpan.FromCenterAndRadius(NUSCentre, Distance.FromKilometers(MEAN_MAP_RADIUS))) {
	                IsShowingUser = true,
	                HeightRequest = 100,
	                WidthRequest = 960,
	                VerticalOptions = LayoutOptions.FillAndExpand
	            };

	        // slider to change radius from 0.1 - 0.9 km (for simulator testing, not needed for device testing)
			var slider = new Slider (1, 9, 5);
			slider.ValueChanged += (sender, e) => {
			    var zoomLevel = e.NewValue; // between 1 and 9
				currRadius = (MEAN_MAP_RADIUS * 2) - (zoomLevel/(MEAN_MAP_RADIUS * 20));
			    CentraliseMap(map.VisibleRegion.Center);
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
			UpdatePublicBusPins ();
			UpdateBusPins ();
			UpdateStopPins ();

	    }

	    public static void CentraliseMap (Xamarin.Forms.Maps.Position pos) {
	    	map.MoveToRegion(MapSpan.FromCenterAndRadius(
				pos, Distance.FromKilometers (currRadius)));
	    }

	    public static void SetFreezeMap (bool value) {
			FreezeMap = value;
	    }

	    private async void UpdateBusPins ()
		{
			while (true) {
				// skip update pins if map is freezed (user clicks on pin)
				if (!FreezeMap) {
					// remove all bus pins
					foreach (CustomPin p in map.BusPins)
						map.Pins.Remove (p.Pin);
					map.BusPins.Clear ();

					foreach (BusOnRoad bor in BusHelper.ActiveBuses.Values) {
						// move bus to next checkpoint on the route for simulation
						// actual deployment: get real-time position of bus
						BusSimulator.GoToNextCheckpoint (bor);

						// add pin to map if service is to be shown on map
						if (BusHelper.BusSvcs [bor.routeName].showOnMap) {
							var description = "Start: " + BusHelper.BusStops [bor.firstStop].name + "\n" +
							                  "End: " + BusHelper.BusStops [bor.lastStop].name + "\n" +
							                  "Approaching: " + BusHelper.BusStops [(string)bor.nextStopEnumerator.Current].name + "\n" +
							                  "In: " + BusHelper.GetArrivalTiming (bor.vehiclePlate) + "\n";
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

					// remove buses which has finished plying
					List<BusOnRoad> finishedBuses = BusHelper.ActiveBuses.Values.Where (bor => bor.finished).ToList ();
					foreach (BusOnRoad bor in finishedBuses)
						BusHelper.ActiveBuses.Remove (bor.vehiclePlate);
				}

				// continue after interval
				await Task.Delay(TimeSpan.FromSeconds (SettingsVars.Variables ["REFRESH_BUS_INTERVAL"].value));
			}
	    }

	    private async void UpdatePublicBusPins ()
		{
			while (true) {
				// skip update pins if map is freezed (user clicks on pin)
				if (!FreezeMap) {
					// remove all bus pins
					foreach (CustomPin p in map.PublicBusPins)
						map.Pins.Remove (p.Pin);
					map.PublicBusPins.Clear ();

					// get list of bus stops to call based on enabled public bus svcs on map
					List<string> busStopCodes = new List<string> ();
					foreach (string serviceNo in BusHelper.PublicBusSvcOnMap)
						busStopCodes = busStopCodes.Union (BusHelper.PublicBusSvcStops [serviceNo]).ToList ();

					// get public bus real-time location (by calling API)
					List<PublicBusOnRoad> publicBuses = new List<PublicBusOnRoad> ();
					foreach (string busStopCode in busStopCodes) {
						// get buses passing by bus stop (ignore same bus)
						var thisPublicBuses = await BusHelper.GetPublicBuses (busStopCode);
						publicBuses.AddRange (thisPublicBuses.Where (b1 => !publicBuses.Any (b2 => b2.IsSameBus (b1, MARGIN_OF_ERROR))));
					}

					// add pin if bus has location
					foreach (PublicBusOnRoad bus in publicBuses.Where(bus => 
								(bus.Latitude.HasValue && !bus.Latitude.Value.Equals(0)) && 
								(bus.Longitude.HasValue && !bus.Longitude.Value.Equals(0)))) {

						var description = "Start: " + BusHelper.PublicBusStopCodeName[bus.OriginatingID] + "\n" +
						                  "End: " + BusHelper.PublicBusStopCodeName[bus.TerminatingID] + "\n";
						var pin = new Pin {
							Type = PinType.Place,
							Position = new Xamarin.Forms.Maps.Position (bus.Latitude.Value, bus.Longitude.Value),
							Label = bus.ServiceNo,
							Address = description
						};
						var busPin = new CustomPin {
							Pin = pin,
							Id = bus.ServiceNo,
							Url = bus.ServiceNo + ".png"
						};
						map.Pins.Add (pin);
						map.PublicBusPins.Add (busPin);
					}
				}

				// continue after interval
				await Task.Delay(TimeSpan.FromSeconds (SettingsVars.Variables ["REFRESH_PUBLIC_BUS_INTERVAL"].value));
			}
	    }

	    private async void UpdateStopPins ()
		{
			while (true) {
				// skip update pins if map is freezed (user clicks on pin)
				if (!FreezeMap) {
					// remove all stop pins
					foreach (CustomPin p in map.StopPins)
						map.Pins.Remove (p.Pin);
					map.StopPins.Clear ();

					// add stop pins, with change in arrival timing
					foreach (BusStop busStop in BusHelper.BusStops.Values) {
						var description = "";
						foreach (string svc in busStop.services) {
							// handle repeated service in bus stop case
							// show timing for both directions
							if (busStop.repeatedServices != null && busStop.repeatedServices.Contains (svc)) {
								description += svc + "(to " + BusHelper.BusStops [BusHelper.BusSvcs [svc].loopStop].name + "): ";
								description += BusHelper.GetArrivalTiming (busStop.busStopCode, svc, "BEFORE") + "\n";
								description += svc + "(to " + BusHelper.BusStops [BusHelper.BusSvcs [svc].lastStop].name + "): ";
								description += BusHelper.GetArrivalTiming (busStop.busStopCode, svc, "AFTER") + "\n";
							} else {
								description += svc + ": " + BusHelper.GetArrivalTiming (busStop.busStopCode, svc) + "\n";
							}
						}

						// get public bus arrival timing for bus stop (if public buses pass by)
						// busStopCode with all digits -> public bus will pass by
						Regex regex = new Regex(@"^\d+$");
						if (regex.IsMatch(busStop.busStopCode))
							description += await BusHelper.GetPublicBusesArrivalTiming (busStop.busStopCode);
							
						var pin = new Pin {
							Type = PinType.Place,
							Position = new Xamarin.Forms.Maps.Position (busStop.latitude, busStop.longitude),
							Label = busStop.name + " - " + busStop.busStopCode,
							Address = description
						};
						var stop = new CustomPin {
							Pin = pin,
							Id = "stop",
							Url = "stop.png"
						};
						map.Pins.Add (pin);
						map.StopPins.Add (stop);
					}
				}

				// continue after interval
				await Task.Delay(TimeSpan.FromSeconds (SettingsVars.Variables ["REFRESH_STOP_INTERVAL"].value));
			}
	    }
	}
}

