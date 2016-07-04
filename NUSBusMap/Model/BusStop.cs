using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public class BusStop
	{
		public string busStopCode { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string name { get; set; }
		public List<string> services { get; set; }
		public List<string> repeatedServices { get; set; } // services which pass by bus stop twice
        public string road { get; set; }

        public override string ToString ()
		{
			return string.Format ("{0} - {1}", busStopCode, name);
		}
	}
}

