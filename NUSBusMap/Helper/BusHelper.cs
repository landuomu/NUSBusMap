using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace NUSBusMap
{
	public static class BusHelper
	{
		// enum for different kind of days for diff bus freq
		public enum Days { WEEKDAY, SATURDAY, SUNDAY };
		public enum Freqs { HIGH, MID, LOW };
		private static double MARGIN_OF_ERROR = 10.0; // in m, max distance allowed between bus and stop to be considered reached stop

		public static Dictionary<string,BusStop> BusStops; // key - bus stop code, value - BusStop object (refer to Model)
		public static Dictionary<string,BusSvc> BusSvcs; // key - route name, value - BusSvc object (refer to Model)
		public static Dictionary<string,BusOnRoad> ActiveBuses; // key - vehicle plate, value - BusOnRoad object (refer to Model)
		public static Dictionary<string,List<string>> PublicBusSvcStops; // key - bus service no, value - list of bus stop code which bus service plies
		public static Dictionary<string,string> PublicBusStopCodeName; // dictionary to map public bus stop code to bus stop name
		public static List<string> PublicBusSvcOnMap; // list of public bus service no to be shown on map, toggled in svc page

		// init dictionaries from json file
		public static void LoadBusData ()
		{
			BusStops = JsonLoader.LoadStops();
			BusSvcs = JsonLoader.LoadSvcs();
			PublicBusSvcStops = JsonLoader.LoadPublicBuses ();
			PublicBusStopCodeName = JsonLoader.LoadPublicBusStops ();

			ActiveBuses = new Dictionary<string,BusOnRoad> ();
			PublicBusSvcOnMap = new List<string> ();
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

			// start/restart timer dispatch
			StartTimerDispatch (svc);
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

			// generate display string (give +- 1 allowance)
			return (time == 0 || time == -1) ? "Arr" : ( (time > 60 || time < -1) ? "--" : time + " min" );
		}

		// return "arr" or "x min" or "not operating" of next and subsequent bus timing
		// of each bus service serving the bus stop
		// to show on bus stop
		// for NUS buses
		public static string GetArrivalTiming (string busStopCode, string routeName, string loop = "" /* optional, "BEFORE" or "AFTER" if repeatedService*/)
		{
			BusSvc svc = BusSvcs [routeName];

			// case not operating
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
						var timeOfDay = GetTimeOfDay (routeName);
						var timeDiff = svc.freq [timeOfDay] - (int)(svc.timerSinceLastDispatch.ElapsedMilliseconds / (1000 * 60));
						if (timeDiff >= 0) nextTiming = timeDiff;
						if (timeDiff + svc.freq [timeOfDay] >= 0) subsequentTiming = timeDiff + svc.freq [timeOfDay];
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

		// call api to get formatted bus arrival timing of all public buses plying the bus stop
		public static async Task<string> GetPublicBusesArrivalTiming (string busStopCode)
		{
			string display = "";
			PublicBusStop pbs = await JsonLoader.LoadPublicBusInfo (busStopCode);
			foreach (PublicBusSvc service in pbs.Services) {
				display += service.ServiceNo + ": ";

				// case not operating
				if (service.Status.Equals ("Not In Operation"))
					display += "not operating\n";

				// get next/subsequent bus timing (give +- 1 min offset for public buses timing)
				if (service.NextBus.EstimatedArrival.HasValue) {
					var nextTiming = ((TimeSpan)(service.NextBus.EstimatedArrival - DateTime.Now)).Minutes;
					if (nextTiming >= -1)
						display += (nextTiming <= 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
				}
				if (service.SubsequentBus.EstimatedArrival.HasValue) {
					var nextTiming = ((TimeSpan)(service.SubsequentBus.EstimatedArrival - DateTime.Now)).Minutes;
					if (nextTiming >= -1) {
						display += " / ";
						display += (nextTiming <= 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
					}
				}
				if (service.SubsequentBus3.EstimatedArrival.HasValue) {
					var nextTiming = ((TimeSpan)(service.SubsequentBus3.EstimatedArrival - DateTime.Now)).Minutes;
					if (nextTiming >= -1) {
						display += " / ";
						display += (nextTiming <= 0) ? "Arr" : ((nextTiming > 60) ? "--" : nextTiming + " min");
					}
				}

				display += "\n";
			}

			return display;
		}

		// call api to get bus arrival timing of particular bus service plying the bus stop
		public static async Task<string> GetPublicBusesArrivalTiming (string busStopCode, string busSvcNo) 
		{
			PublicBusStop pbs = await JsonLoader.LoadPublicBusInfo (busStopCode, busSvcNo);
			PublicBusSvc service = pbs.Services [0];

			// case not operating
			if (service.Status.Equals ("Not In Operation"))
				return "not operating";

			// return next/subsequent/subsequent3 timing
			return (service.NextBus.EstimatedArrival.HasValue) ? ((TimeSpan)(service.NextBus.EstimatedArrival - DateTime.Now)).Minutes.ToString() : 
					((service.SubsequentBus.EstimatedArrival.HasValue) ? ((TimeSpan)(service.SubsequentBus.EstimatedArrival - DateTime.Now)).Minutes.ToString() : 
					((service.SubsequentBus3.EstimatedArrival.HasValue) ? ((TimeSpan)(service.SubsequentBus3.EstimatedArrival - DateTime.Now)).Minutes.ToString() : "--"));
		}

		public static async Task<List<PublicBusOnRoad>> GetPublicBuses (string busStopCode) {
			List<PublicBusOnRoad> publicBuses = new List<PublicBusOnRoad> ();
			PublicBusStop pbs = await JsonLoader.LoadPublicBusInfo (busStopCode);

			foreach(PublicBusSvc service in pbs.Services) {
				// case not operating, or public bus not shown on map -- ignore
				if (service.Status.Equals ("Not In Operation") || !PublicBusSvcOnMap.Contains(service.ServiceNo))
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
			int day = GetDayOfWeek ();

			// case not operating throughout the day
			if (svc.firstBusTime.Count <= day)
				return false;

			return currTimeSpan.CompareTo (TimeSpan.Parse (svc.firstBusTime[day])) > 0 &&
				currTimeSpan.CompareTo (TimeSpan.Parse (svc.lastBusTime[day])) < 0;
		}

		// return true if bus stop code / bus service no is public
		public static bool IsPublic(string name) {
			Regex allDigits = new Regex(@"^\d+$");
			return allDigits.IsMatch (name);
		}

		// return int of enum based on current day
		private static int GetDayOfWeek() {
			DateTime now = DateTime.Now;
			if (now.DayOfWeek.Equals (DayOfWeek.Saturday))
				return (int)Days.SATURDAY;
			else if (now.DayOfWeek.Equals (DayOfWeek.Sunday))
				return (int)Days.SUNDAY;
			else
				return (int)Days.WEEKDAY;
		}

		// return int of freqs enum based on time of day and bus service
		public static int GetTimeOfDay(string routeName) {
			// one freq case (express services)
			if (BusHelper.BusSvcs [routeName].freq.Count == 1)
				return (int)Freqs.HIGH;

			DateTime now = DateTime.Now;
			DayOfWeek day = now.DayOfWeek;
			TimeSpan time = now.TimeOfDay;

			// case A1/A2/D1/D2
			if (routeName[0].Equals('A') || routeName[0].Equals('D')) {
				if (day.Equals (DayOfWeek.Sunday) || (day.Equals (DayOfWeek.Saturday) && time > new TimeSpan (20, 0, 0)))
					return (int)Freqs.LOW;

				TimeSpan[] peakStarts = { new TimeSpan(8,0,0), new TimeSpan(12,0,0), new TimeSpan(17,0,0) };
				TimeSpan[] peakEnds = { new TimeSpan(10,0,0), new TimeSpan(14,0,0), new TimeSpan(20,0,0) };
				for (int i=0;i<peakStarts.Length;i++) 
					if (time > peakStarts [i] && time < peakEnds [i])
						return (int)Freqs.HIGH;

				return (int)Freqs.MID;

			} else if (routeName.Equals("BTC")) {
				if (day.Equals (DayOfWeek.Saturday))
					return (int)Freqs.LOW;

				TimeSpan[] peakStarts = { new TimeSpan(7,20,0), new TimeSpan(12,0,0), new TimeSpan(17,0,0) };
				TimeSpan[] peakEnds = { new TimeSpan(9,0,0), new TimeSpan(14,0,0), new TimeSpan(20,0,0) };
				for (int i=0;i<peakStarts.Length;i++) 
					if (time > peakStarts [i] && time < peakEnds [i])
						return (int)Freqs.HIGH;

				return (int)Freqs.MID;
			} else {
				// case B/C, only two freqs
				return (day.Equals (DayOfWeek.Saturday) || time > new TimeSpan (19, 0, 0)) ? 
						(int)Freqs.MID : (int)Freqs.HIGH;
			}
		}

		// have a stopwatch to keep track of time since bus service last dispatched
		// in order to estimate bus arrival timing for first stop
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
	}
}

