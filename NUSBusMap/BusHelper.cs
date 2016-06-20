using System;
using System.Collections.Generic;

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
				speed = 5.0
			};
			ActiveBuses.Add (vehiclePlate, bus);
		}

		public static bool RemoveBusOnRoad (string vehiclePlate) {
			return ActiveBuses.Remove (vehiclePlate);
		}

		// return "arr" or "x min" or "not operating"
		public static string GetArrivalTiming (string busStopCode, string routeName) {
			if (!IsWithinServiceTiming(routeName))
				return "not operating";

			// calculate arrival timing based on bus stop and route
			Position stopPos = new Position ();
			stopPos.Latitude = BusStops [busStopCode].latitude;
			stopPos.Longitude = BusStops [busStopCode].longitude;

			int min = Int16.MaxValue;
			foreach (BusOnRoad bor in ActiveBuses.Values) {
				if (bor.routeName.Equals(routeName)) {
					Position busPos = new Position ();
					busPos.Latitude = bor.latitude;
					busPos.Longitude = bor.longitude;

					// TODO: think of a better way to calculate
				}
			}
			return "3 min";
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

