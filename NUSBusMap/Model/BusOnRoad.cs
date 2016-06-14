using System;
using System.Collections;

namespace NUSBusMap
{
	public class BusOnRoad
	{
		public string vehiclePlate { get; set; }
		public string routeName { get; set; }
		public int firstStop { get; set; }
		public int lastStop { get; set; }
		public IEnumerator nextStopEnumerator { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
	}
}

