using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public class BusStop
	{
		public string busStopCode { get; set; } // primary key
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string name { get; set; }
		public List<string> services { get; set; }
		public List<string> publicServices { get; set; } // separate list for public buses, if no public bus still need to declare empty list
		public List<string> repeatedServices { get; set; } // services which pass by bus stop twice
        public string road { get; set; }
        // not in json
        public bool alertEnabled { get; set; } // determine if need to check alert for bus stop
	}
}

