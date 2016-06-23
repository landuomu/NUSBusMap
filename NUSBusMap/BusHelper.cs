using System;
using System.Collections.Generic;
using System.Linq;

using XLabs.Platform.Device;
using XLabs.Platform;
using XLabs.Ioc;
using XLabs.Platform.Services.Geolocation; // to calculate position

namespace NUSBusMap
{
	public static class BusHelper
	{
		public enum Days { WEEKDAY, SATURDAY, SUNDAY };

		public static Dictionary<string,BusStop> BusStops;
		public static Dictionary<string,BusSvc> BusSvcs;
		public static Dictionary<string,BusOnRoad> ActiveBuses;

		public static void LoadBusData ()
		{
			BusStops = JsonLoader.LoadStops();
			BusSvcs = JsonLoader.LoadSvcs();
			ActiveBuses = new Dictionary<string,BusOnRoad> ();
		}

		// TODO: helper methods to extract specific bus data

		// add bus when bus starts to ply on road
		public static void AddBusOnRoad (string vehiclePlate, string routeName) {
			var svc = BusSvcs[routeName];
			var stopEnum = svc.stops.GetEnumerator();

			// start from the second bus stop for next stop
			if (!stopEnum.MoveNext ())
				return;
			stopEnum.MoveNext ();

			var bus = new BusOnRoad {
				vehiclePlate = vehiclePlate,
				routeName = routeName,
				firstStop = svc.firstStop,
				lastStop = svc.lastStop,
				nextStopEnumerator = stopEnum,
				// take firstStop position as initial bus position
				latitude = BusStops[svc.firstStop].latitude,
				longitude = BusStops[svc.firstStop].longitude,
				avgSpeed = 5.0,
				currSpeed = 5.0
			};
			ActiveBuses.Add (vehiclePlate, bus);
		}

		// remove bus when bus reaches last stop
		public static bool RemoveBusOnRoad (string vehiclePlate) {
			return ActiveBuses.Remove (vehiclePlate);
		}

		// return "arr" or "x min" of bus reaching next stop
		// to show on bus on road
		public static string GetArrivalTiming (string vehiclePlate) {
			BusOnRoad bor = ActiveBuses [vehiclePlate];
			BusSvc svc = BusSvcs [bor.routeName];

			// get diff of distance travelled by bus and distance between stops for the service
			var diffDist = svc.distanceBetweenStops [svc.stops.IndexOf ((string)bor.nextStopEnumerator.Current) - 1] - bor.distanceTravelled;
			var time = (int)((diffDist / bor.avgSpeed) / 60); // in min

			return (time == 0) ? "Arr" : ( (time > 30) ? "--" : time + " min" );
		}

		// return "arr" or "x min" or "not operating" of next and subsequent bus timing
		// to show on bus stop
		public static string GetArrivalTiming (string busStopCode, string routeName)
		{
			BusSvc svc = BusSvcs [routeName];
			BusStop stop = BusStops [busStopCode];

			if (!IsWithinServiceTiming (routeName))
				return "not operating";

			// calculate arrival timing (next and subsequent) based on bus stop and route
			int nextTiming = Int16.MaxValue;
			int subsequentTiming = Int16.MaxValue;
			foreach (BusOnRoad bor in ActiveBuses.Values.Where(b => b.routeName.Equals(routeName))) {
				if (busStopCode.Equals (bor.firstStop)) {
					if (svc.timerSinceLastDispatch != null) {
						// get time by freq for the first stop
						var timeDiff = svc.freq [(int)Days.WEEKDAY] - (int)(svc.timerSinceLastDispatch.ElapsedMilliseconds / (1000 * 60));
						nextTiming = timeDiff;
						subsequentTiming = timeDiff + svc.freq [(int)Days.WEEKDAY];
					}
				} else {
					// get diff of distance travelled by bus and distance between stops for the service
					var diffDist = svc.distanceBetweenStops [svc.stops.IndexOf (busStopCode) - 1] - bor.distanceTravelled;

					// if arrived bus stop
					if (diffDist < SettingsVars.MARGIN_OF_ERROR
					    && ((string)bor.nextStopEnumerator.Current).Equals (busStopCode)) {
					    // shift next stop indicator
						bor.nextStopEnumerator.MoveNext ();
						// TODO: simulate bus stopping
					}

					// ignore getting time if bus passed stop
					if (diffDist < 0)
						continue;

					var time = (int)((diffDist / bor.avgSpeed) / 60); // in min
					// store the next and subsequent time if lesser
					if (nextTiming > time) {
						subsequentTiming = nextTiming;
						nextTiming = time;
					} else if (subsequentTiming > time) {
						subsequentTiming = time;
					}
				}
			}

			// generate display string of next and subsequent timings (ignore if more than 30 min)
			string display = "";
			display += (nextTiming == 0) ? "Arr" : ((nextTiming > 30) ? "--" : nextTiming + " min");
			display += " / ";
			display += (subsequentTiming == 0) ? "Arr" : (subsequentTiming > 30) ? "--" : subsequentTiming + " min";
			return display;
		}

		public static bool IsWithinServiceTiming(string routeName) {
			DateTime now = DateTime.Now;
			TimeSpan currTimeSpan = new TimeSpan (now.Hour, now.Minute, now.Second);
			BusSvc svc = BusSvcs [routeName];
			return currTimeSpan.CompareTo (TimeSpan.Parse (svc.firstBusTime[(int)Days.WEEKDAY])) > 0 &&
			currTimeSpan.CompareTo (TimeSpan.Parse (svc.lastBusTime[(int)Days.WEEKDAY])) < 0;
		}
	}
}

