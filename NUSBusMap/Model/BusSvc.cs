using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public class BusSvc
	{
		public string routeName { get; set; }
		public List<string> firstBusTime { get; set; } // [weekday, sat, sun/ph] in 24h format
		public List<string> lastBusTime { get; set; } // [weekday, sat, sun/ph] in 24h formst
		public int firstStop { get; set; }
		public int lastStop { get; set; }
		public List<int> stops { get; set; }
		public bool showOnMap { get; set; }
	}
}

