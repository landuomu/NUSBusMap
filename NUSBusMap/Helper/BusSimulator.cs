using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace NUSBusMap
{
	public static class BusSimulator
	{
		// functions to simulate bus plying on road without actual real-time data
		// functions which will not be used during actual deployment

//		private static double avgSpeedPeak = 4.0; // in m/s
//		private static double avgSpeedNonPeak = 5.0; // in m/s
//		private static double avgBoardingTimePeak = 30; // in secs
//		private static double avgBoardingTimeNonPeak = 15; // in secs

		public static void DispatchBuses ()
		{
			// set timer for each bus service to dispatch bus at freq (if within service timing)
			foreach (BusSvc bs in BusHelper.BusSvcs.Values) {
				// kickstart first time
				if (BusHelper.IsWithinServiceTiming (bs.routeName)) {
					BusHelper.AddBusOnRoad (bs.routeName + "-" + BusHelper.ActiveBuses.Count, bs.routeName);
					StartTimerDispatch (bs);
				}

				Device.StartTimer (TimeSpan.FromMinutes (bs.freq [(int)BusHelper.Days.WEEKDAY]), () => {
					if (BusHelper.IsWithinServiceTiming(bs.routeName)) {
						BusHelper.AddBusOnRoad(bs.routeName + "-" + BusHelper.ActiveBuses.Count, bs.routeName);
						return true;
					} else 
						return false;
				});
			}
		}

		public static void GoToNextCheckpoint (BusOnRoad bor)
		{
			BusSvc svc = BusHelper.BusSvcs [bor.routeName];
			if (bor.nextCheckpointEnumerator == null)
				bor.nextCheckpointEnumerator = svc.checkpoints.GetEnumerator ();
			else if (bor.nextDistanceEnumerator == null)
				bor.nextDistanceEnumerator = svc.distanceBetweenCheckpoints.GetEnumerator ();

			double longitude = BusHelper.BusStops [svc.firstStop].longitude;
			double latitude = BusHelper.BusStops [svc.firstStop].latitude;

			// update position and distance based on checkpoint
			if (bor.nextCheckpointEnumerator.MoveNext ())
				longitude = (double)bor.nextCheckpointEnumerator.Current;
			if (bor.nextCheckpointEnumerator.MoveNext ())
				latitude = (double)bor.nextCheckpointEnumerator.Current;
			if (bor.nextDistanceEnumerator != null && bor.nextDistanceEnumerator.MoveNext ())
				bor.distanceTravelled += (double)bor.nextDistanceEnumerator.Current;
			

			bor.longitude = longitude;
			bor.latitude = latitude;
		}

		private static void StartTimerDispatch (BusSvc bs)
		{
			// init/reset stopwatch
			if (bs.timerSinceLastDispatch == null) {
				bs.timerSinceLastDispatch = new Stopwatch ();
				bs.timerSinceLastDispatch.Start ();
			} else {
				bs.timerSinceLastDispatch.Restart ();
			}
		}


		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//:::                                                                         :::
		//:::  This routine calculates the distance between two points (given the     :::
		//:::  latitude/longitude of those points). It is being used to calculate     :::
		//:::  the distance between two locations using GeoDataSource(TM) products    :::
		//:::                                                                         :::
		//:::  Definitions:                                                           :::
		//:::    South latitudes are negative, east longitudes are positive           :::
		//:::                                                                         :::
		//:::  Passed to function:                                                    :::
		//:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
		//:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
		//:::    unit = the unit you desire for results                               :::
		//:::           where: 'M' is statute miles (default)                         :::
		//:::                  'K' is kilometers                                      :::
		//:::                  'N' is nautical miles                                  :::
		//:::                                                                         :::
		//:::  Worldwide cities and other features databases with latitude longitude  :::
		//:::  are available at http://www.geodatasource.com                          :::
		//:::                                                                         :::
		//:::  For enquiries, please contact sales@geodatasource.com                  :::
		//:::                                                                         :::
		//:::  Official Web site: http://www.geodatasource.com                        :::
		//:::                                                                         :::
		//:::           GeoDataSource.com (C) All Rights Reserved 2015                :::
		//:::                                                                         :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		private static double distance(double lat1, double lon1, double lat2, double lon2) {
		  double theta = lon1 - lon2;
		  double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
		  dist = Math.Acos(dist);
		  dist = rad2deg(dist);
		  dist = dist * 60 * 1.1515;
		  dist = dist * 1.609344;
		  return (dist * 1000); // km to m
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts decimal degrees to radians             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private static double deg2rad(double deg) {
		  return (deg * Math.PI / 180.0);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts radians to decimal degrees             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private static double rad2deg(double rad) {
		  return (rad / Math.PI * 180.0);
		}
	}
}

