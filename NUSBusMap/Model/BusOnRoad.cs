using System;
using System.Collections;

namespace NUSBusMap
{
	public class BusOnRoad
	{
		public string vehiclePlate { get; set; }
		public string routeName { get; set; }
		public string firstStop { get; set; }
		public string lastStop { get; set; }
		public IEnumerator nextStopEnumerator { get; set; }
		public IEnumerator nextCheckpointEnumerator { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public double speed { get; set; }
	}
}

