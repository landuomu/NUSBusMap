﻿using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace NUSBusMap
{
	public class BusMap : Map
	{
		// add lists of custom pins in addition to xamarin map
		public List<CustomPin> BusPins { get; set; }
		public List<CustomPin> PublicBusPins { get; set; }
		public List<CustomPin> StopPins { get; set; }

		public BusMap(MapSpan ms) : base(ms) {
			BusPins = new List<CustomPin> ();
			PublicBusPins = new List<CustomPin> ();
			StopPins = new List<CustomPin> ();
		}
	}
}

