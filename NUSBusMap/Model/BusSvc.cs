using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NUSBusMap
{
	public class BusSvc
	{
		public string routeName { get; set; }
		public List<string> firstBusTime { get; set; } // [weekday, sat, sun/ph] in "hh:mm" format
		public List<string> lastBusTime { get; set; } // [weekday, sat, sun/ph] in "hh:mm" format
		public List<int> freq { get; set; } // [high, mid, low] in mins
		public string firstStop { get; set; }
		public string lastStop { get; set; }
		public string loopStop { get; set; }
		public List<double> distanceBetweenStops { get; set; } // distance travelled from firstStop
		public List<string> stops { get; set; } // bus stops which service plies, store the busStopCode of stops
		public List<double> distanceBetweenCheckpoints { get; set; } // for busi simulation purpose
		public List<double> checkpoints { get; set; } // data of long,lat of points the route passes by, for bus simulation (testing) and route drawing (in future)

		// not in json
		public bool showOnMap { get; set; }
		public Stopwatch timerSinceLastDispatch { get; set; }
	}
}

