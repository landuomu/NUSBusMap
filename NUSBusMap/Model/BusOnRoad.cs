using System;
using System.Collections;

namespace NUSBusMap
{
	public class BusOnRoad
	{
		public string vehiclePlate { get; set; } // primary key
		public string routeName { get; set; } // foreign key
		public string firstStop { get; set; }
		public string lastStop { get; set; }
		public string loopStop { get; set; }
		public int stopCounter { get; set; } // # of stops passed

		// enumerator, init from BusSvcs[routeName]
		public IEnumerator nextStopEnumerator { get; set; } // bus stop code of next stop
		public IEnumerator nextCheckpointEnumerator { get; set; } // long/lat of next checkpoint
		public IEnumerator nextDistanceEnumerator { get; set; } // distance from first stop to the next stop

		public double latitude { get; set; }
		public double longitude { get; set; }
		public double avgSpeed { get; set; }
		public double currSpeed { get; set; }
		public double distanceTravelled { get; set; }
		public bool finished { get; set; }
	}
}

