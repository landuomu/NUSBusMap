using System;
using System.Threading.Tasks;

namespace NUSBusMap
{
	public static class BusSimulator
	{
		// functions to simulate bus plying on road without actual real-time data
		// functions which will not be used during actual deployment

		// simulate bus dispatched at first stop to start service
		public static void DispatchBuses ()
		{
			foreach (BusSvc bs in BusHelper.BusSvcs.Values) {
				// set timer for each bus service to dispatch bus at freq (if within service timing)
				DispatchBasedOnTime (bs);
			}
		}

		// simulate bus moving along the route (via checkpoints on route)
		public static void GoToNextCheckpoint (BusOnRoad bor)
		{
			// init if needed
			BusSvc svc = BusHelper.BusSvcs [bor.routeName];
			if (bor.nextCheckpointEnumerator == null)
				bor.nextCheckpointEnumerator = svc.checkpoints.GetEnumerator ();
			else if (bor.nextDistanceEnumerator == null)
				bor.nextDistanceEnumerator = svc.distanceBetweenCheckpoints.GetEnumerator ();

			// default position at first stop
			double longitude = BusHelper.BusStops [svc.firstStop].longitude;
			double latitude = BusHelper.BusStops [svc.firstStop].latitude;

			// update position and distance based on checkpoint
			if (bor.nextCheckpointEnumerator.MoveNext ())
				longitude = (double)bor.nextCheckpointEnumerator.Current;
			if (bor.nextCheckpointEnumerator.MoveNext ())
				latitude = (double)bor.nextCheckpointEnumerator.Current;
			if (bor.nextDistanceEnumerator != null && bor.nextDistanceEnumerator.MoveNext ())
				bor.distanceTravelled += (double)bor.nextDistanceEnumerator.Current;
			
			// update bus position
			bor.longitude = longitude;
			bor.latitude = latitude;
		}

		private static async void DispatchBasedOnTime (BusSvc bs)
		{
			while (BusHelper.IsWithinServiceTiming (bs.routeName)) {
				BusHelper.AddBusOnRoad (bs.routeName + "-" + BusHelper.ActiveBuses.Count, bs.routeName);

				// dispatch again after freq
				await Task.Delay (TimeSpan.FromMinutes (bs.freq [BusHelper.GetTimeOfDay (bs.routeName)]));
			}
		}
	}
}

