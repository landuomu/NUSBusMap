using System;
using Xamarin.Forms;

namespace NUSBusMap
{
	public static class BusSimulator
	{
		private enum Days { WEEKDAY, SATURDAY, SUNDAY };
//		private static double avgSpeedPeak = 4.0; // in m/s
//		private static double avgSpeedNonPeak = 5.0; // in m/s
//		private static double avgBoardingTimePeak = 30; // in secs
//		private static double avgBoardingTimeNonPeak = 15; // in secs

		public static void DispatchBuses() {
			// set timer for each bus service to dispatch bus at freq (if within service timing)
			foreach (BusSvc bs in BusHelper.BusSvcs.Values) {
				Device.StartTimer (TimeSpan.FromMinutes (bs.freq [(int)Days.WEEKDAY]), () => {
					BusHelper.AddBusOnRoad(bs.routeName + "-" + BusHelper.ActiveBuses.Count, bs.routeName);

					return IsWithinServiceTiming(bs.routeName);
				});
			}
		}

		private static bool IsWithinServiceTiming(string routeName) {
			DateTime now = DateTime.Now;
			TimeSpan currTimeSpan = new TimeSpan (now.Hour, now.Minute, now.Second);
			return currTimeSpan.CompareTo (TimeSpan.Parse (BusHelper.BusSvcs [routeName].firstBusTime[(int)Days.WEEKDAY])) > 0 &&
			currTimeSpan.CompareTo (TimeSpan.Parse (BusHelper.BusSvcs [routeName].lastBusTime[(int)Days.WEEKDAY])) < 0;
		}

		public static void GoToNextCheckpoint (BusOnRoad bor)
		{
			var svc = BusHelper.BusSvcs [bor.routeName];
			if (bor.nextCheckpointEnumerator == null)
				bor.nextCheckpointEnumerator = svc.checkpoints.GetEnumerator ();

			double longitude = BusHelper.BusStops [svc.firstStop].longitude;
			double latitude = BusHelper.BusStops [svc.firstStop].latitude;

			// update position based on checkpoint
			if (bor.nextCheckpointEnumerator.MoveNext ())
				longitude = (double)bor.nextCheckpointEnumerator.Current;
			else {
				BusHelper.RemoveBusOnRoad (bor.vehiclePlate);
				return;
			}
			if (bor.nextCheckpointEnumerator.MoveNext ())
				latitude = (double)bor.nextCheckpointEnumerator.Current;

			bor.longitude = longitude;
			bor.latitude = latitude;
		}
	}
}

