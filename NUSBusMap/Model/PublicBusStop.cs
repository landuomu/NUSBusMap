using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public class PublicBusStop
	{
		public string BusStopID { get; set; }
		public List<PublicBusSvc> Services { get; set; }
	}
}

