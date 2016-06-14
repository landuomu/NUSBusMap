using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public static class BusHelper
	{
		public static Dictionary<int,BusStop> BusStops;
		public static Dictionary<string,BusSvc> BusSvcs;

		public static void LoadBusData ()
		{
			BusStops = JsonLoader.LoadStops();
			BusSvcs = JsonLoader.LoadSvcs();
		}

		// TODO: helper methods to extract specific bus data


	}
}

