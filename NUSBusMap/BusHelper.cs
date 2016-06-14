using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public static class BusHelper
	{
		public static List<BusStop> BusStops;
		public static List<BusSvc> BusSvcs;

		public static void LoadBusData ()
		{
			BusStops = (List<BusStop>) JsonLoader.LoadStops();
			BusSvcs = (List<BusSvc>) JsonLoader.LoadSvcs();
		}

		// TODO: helper methods to extract specific bus data
	}
}

