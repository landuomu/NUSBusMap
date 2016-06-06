using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public class BusStop
	{
		public int busStopCode { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string name { get; set; }
		public List<BusSvc> services { get; set; }
	}
}

