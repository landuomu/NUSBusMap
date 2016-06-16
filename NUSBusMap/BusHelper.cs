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
				speed = 10.0
			};
			ActiveBuses.Add (vehiclePlate, bus);
		}

		public static bool RemoveBusOnRoad (string vehiclePlate) {
			return ActiveBuses.Remove (vehiclePlate);
		}

		public static int GetArrivalTiming (string busStopCode, string routeName) {
			// TODO: calculate arrival timing based on bus stop and route
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
			return 3;
		}

		public static void GoToNextCheckpoint (BusOnRoad bor)
		{
			var svc = BusSvcs [bor.routeName];
			if (bor.nextCheckpointEnumerator == null)
				bor.nextCheckpointEnumerator = svc.checkpoints.GetEnumerator ();

			double longitude = BusStops [svc.firstStop].longitude;
			double latitude = BusStops [svc.firstStop].latitude;

			// update position based on checkpoint
			if (bor.nextCheckpointEnumerator.MoveNext ())
				longitude = (double)bor.nextCheckpointEnumerator.Current;
			else {
				RemoveBusOnRoad (bor.vehiclePlate);
				return;
			}
			if (bor.nextCheckpointEnumerator.MoveNext ())
				latitude = (double)bor.nextCheckpointEnumerator.Current;

			bor.longitude = longitude;
			bor.latitude = latitude;
		}
	}
}

