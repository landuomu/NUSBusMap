using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NUSBusMap
{
	public static class BusHelper
	{
		// enum for different kind of days for diff bus freq
		public enum Days { WEEKDAY, SATURDAY, SUNDAY };
		private static double MARGIN_OF_ERROR = 10.0; // in m, max distance allowed between bus and stop to be considered reached stop

		public static Dictionary<string,BusStop> BusStops;
		public static Dictionary<string,BusSvc> BusSvcs;
		public static Dictionary<string,BusOnRoad> ActiveBuses;

		// init dictionaries
		public static void LoadBusData ()
		{
			BusStops = JsonLoader.LoadStops();
			BusSvcs = JsonLoader.LoadSvcs();
			ActiveBuses = new Dictionary<string,BusOnRoad> ();
		}

		// add bus when bus starts to ply on road
		public static void AddBusOnRoad (string vehiclePlate, string routeName) {
			var svc = BusSvcs[routeName];
			var stopEnum = svc.stops.GetEnumerator();

			// start from the second bus stop for next stop
			if (!stopEnum.MoveNext ())
				return;
			stopEnum.MoveNext ();

			// construct bor object
			var bor = new BusOnRoad {
				vehiclePlate = vehiclePlate,
				routeName = routeName,
				firstStop = svc.firstStop,
				lastStop = svc.lastStop,
				loopStop = svc.loopStop,
				stopCounter = 0,
				nextStopEnumerator = stopEnum,
				// take firstStop position as initial bus position
				latitude = BusStops[svc.firstStop].latitude,
				longitude = BusStops[svc.firstStop].longitude,
				avgSpeed = 5.0,
				currSpeed = 5.0,
				distanceTravelled = 0.0,
				finished = false
			};
			ActiveBuses.Add (vehiclePlate, bor);
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
			string busStopCode = (string)bor.nextStopEnumerator.Current;

			// get diff of distance travelled by bus and distance between stops for the service
			var diffDist = svc.distanceBetweenStops [bor.stopCounter] - bor.distanceTravelled;
			var time = (int)((diffDist / bor.avgSpeed) / 60); // in min

			// if arrived bus stop
			if (diffDist < MARGIN_OF_ERROR
			    && ((string)bor.nextStopEnumerator.Current).Equals (busStopCode)) {
			    // shift next stop indicator
			    // if no more stop, finish service
			    // else, keep counting
				if (!bor.nextStopEnumerator.MoveNext ())
					bor.finished = true;
				else 
					bor.stopCounter++;
			}

			// generate display string
			return (time == 0) ? "Arr" : ( (time > 60) ? "--" : time + " min" );
		}

		// return "arr" or "x min" or "not operating" of next and subsequent bus timing
		// of each bus service serving the bus stop
		// to show on bus stop
		public static string GetArrivalTiming (string busStopCode, string routeName, string loop = "" /* optional, "BEFORE" or "AFTER" if repeatedService*/)
		{
			BusSvc svc = BusSvcs [routeName];
			BusStop stop = BusStops [busStopCode];

			if (!IsWithinServiceTiming (routeName))
				return "not operating";

			// calculate arrival timing (next and subsequent) based on bus stop and route
			int nextTiming = Int16.MaxValue;
			int subsequentTiming = Int16.MaxValue;
			foreach (BusOnRoad bor in ActiveBuses.Values.Where(b => b.routeName.Equals(routeName))) {
				// ignore buses of wrong direction for repeated services case
				if ((loop.Equals ("BEFORE") && bor.stopCounter > svc.stops.IndexOf (svc.loopStop)) ||
				    (loop.Equals ("AFTER") && bor.stopCounter < svc.stops.IndexOf (svc.loopStop)))
					continue;

				// first bus stop case
				if (busStopCode.Equals (bor.firstStop)) {
					if (bor.stopCounter == 0 && svc.timerSinceLastDispatch != null) {
						// get time by freq for the first stop (ignore negative)
						var timeDiff = svc.freq [(int)Days.WEEKDAY] - (int)(svc.timerSinceLastDispatch.ElapsedMilliseconds / (1000 * 60));
						if (timeDiff >= 0) nextTiming = timeDiff;
						if (timeDiff + svc.freq [(int)Days.WEEKDAY] >= 0) subsequentTiming = timeDiff + svc.freq [(int)Days.WEEKDAY];
					}
				} else {
					// get diff of distance travelled by bus and distance between stops for the service
					// count from back for case after loop (repeated service) or last stop
					var index = (loop.Equals("AFTER") || bor.stopCounter >= svc.stops.Count - 2) ? svc.stops.LastIndexOf (busStopCode) - 1 : svc.stops.IndexOf(busStopCode) - 1;
					// bounds check
					if (index < 0 || index >= svc.distanceBetweenStops.Count)
						continue;
					var diffDist = svc.distanceBetweenStops [index] - bor.distanceTravelled;

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
			display += (nextTiming == 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
			display += " / ";
			display += (subsequentTiming == 0) ? "Arr" : (subsequentTiming > 60) ? "--" : subsequentTiming + " min";
			return display;
		}

		public static async Task<string> GetPublicBusesArrivalTiming (string busStopCode)
		{
			string display = "";
			PublicBusStop pbs = await JsonLoader.LoadPublicBusInfo (busStopCode);
			foreach (PublicBusSvc service in pbs.Services) {
				display += service.ServiceNo + ": ";

				// case not operating
				if (service.Status.Equals ("Not In Operation"))
					display += "not operating\n";

				// get next/subsequent bus timing
				if (service.NextBus.EstimatedArrival.HasValue) {
					var nextTiming = ((TimeSpan)(service.NextBus.EstimatedArrival - DateTime.Now)).Minutes;
					if (nextTiming >= 0)
						display += (nextTiming == 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
				}
				if (service.SubsequentBus.EstimatedArrival.HasValue) {
					var nextTiming = ((TimeSpan)(service.SubsequentBus.EstimatedArrival - DateTime.Now)).Minutes;
					if (nextTiming >= 0) {
						display += " / ";
						display += (nextTiming == 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
					}
				}
				if (service.SubsequentBus3.EstimatedArrival.HasValue) {
					var nextTiming = ((TimeSpan)(service.SubsequentBus3.EstimatedArrival - DateTime.Now)).Minutes;
					if (nextTiming >= 0) {
						display += " / ";
						display += (nextTiming == 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
					}
				}

				display += "\n";
			}

			return display;
		}

		public static async Task<List<PublicBusOnRoad>> GetPublicBuses (string busStopCode) {
			List<PublicBusOnRoad> publicBuses = new List<PublicBusOnRoad> ();
			PublicBusStop pbs = await JsonLoader.LoadPublicBusInfo (busStopCode);

			foreach(PublicBusSvc service in pbs.Services) {
				// case not operating -- ignore
				if (service.Status.Equals ("Not In Operation"))
					continue;

				// add to list after adding service info to bus (for display purpose)
				service.NextBus.ServiceNo = Regex.Replace (service.ServiceNo, @"[^\d]", String.Empty); // ignore alphabet in serviceno when displaying bus position
				service.SubsequentBus.ServiceNo = Regex.Replace (service.ServiceNo, @"[^\d]", String.Empty);
				service.SubsequentBus3.ServiceNo = Regex.Replace (service.ServiceNo, @"[^\d]", String.Empty);
				service.NextBus.OriginatingID = service.OriginatingID;
				service.SubsequentBus.OriginatingID = service.OriginatingID;
				service.SubsequentBus3.OriginatingID = service.OriginatingID;
				service.NextBus.TerminatingID = service.TerminatingID;
				service.SubsequentBus.TerminatingID = service.TerminatingID;
				service.SubsequentBus3.TerminatingID = service.TerminatingID;

				publicBuses.Add (service.NextBus);
				publicBuses.Add (service.SubsequentBus);
				publicBuses.Add (service.SubsequentBus3);
			}

			return publicBuses;
		}

		// return true if current time between first bus and last bus timing
		public static bool IsWithinServiceTiming(string routeName) {
			TimeSpan currTimeSpan = DateTime.Now.TimeOfDay;
			BusSvc svc = BusSvcs [routeName];
			return currTimeSpan.CompareTo (TimeSpan.Parse (svc.firstBusTime[GetPeriodOfDay()])) > 0 &&
				currTimeSpan.CompareTo (TimeSpan.Parse (svc.lastBusTime[GetPeriodOfDay()])) < 0;
		}

		// return int of enum based on current day
		public static int GetPeriodOfDay() {
			DateTime now = DateTime.Now;
			if (now.DayOfWeek.Equals (DayOfWeek.Saturday))
				return (int)Days.SATURDAY;
			else if (now.DayOfWeek.Equals (DayOfWeek.Sunday))
				return (int)Days.SUNDAY;
			else
				return (int)Days.WEEKDAY;
		}
	}
}

