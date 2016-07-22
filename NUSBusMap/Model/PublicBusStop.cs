using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public class PublicBusStop
	{
		// model based on LTA DataMall API
		public string BusStopID { get; set; } // primary key
		public List<PublicBusSvc> Services { get; set; }
	}
}

