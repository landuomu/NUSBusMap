using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public static class BusHelper
	{
		public static Dictionary<int,BusStop> BusStops;
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
				longitude = BusStops[svc.firstStop].longitude
			};
			ActiveBuses.Add (vehiclePlate, bus);
		}

		public static bool RemoveBusOnRoad (string vehiclePlate) {
			return ActiveBuses.Remove (vehiclePlate);
		}

		public static int GetArrivalTiming (int busStopCode, string routeName) {
			// TODO: calculate arrival timing based on bus stop and route
			return 3;
		}

		public static void GoToNextCheckpoint (string vehiclePlate, string routeName) {
			if (ActiveBuses [vehiclePlate].nextCheckpointEnumerator == null)
				ActiveBuses [vehiclePlate].nextCheckpointEnumerator = BusSvcs [routeName].checkpoints.GetEnumerator ();

			double longitude = BusStops[BusSvcs[routeName].firstStop].longitude;
			double latitude = BusStops[BusSvcs[routeName].firstStop].latitude;

			if (ActiveBuses [vehiclePlate].nextCheckpointEnumerator.MoveNext ())
				longitude = (double)ActiveBuses [vehiclePlate].nextCheckpointEnumerator.Current;
			if (ActiveBuses [vehiclePlate].nextCheckpointEnumerator.MoveNext ())
				latitude = (double)ActiveBuses [vehiclePlate].nextCheckpointEnumerator.Current;

			ActiveBuses [vehiclePlate].longitude = longitude;
			ActiveBuses [vehiclePlate].latitude = latitude;
		}
	}
}

