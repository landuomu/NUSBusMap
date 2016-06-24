using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace NUSBusMap
{
	public class BusMap : Map
	{
		public List<CustomPin> BusPins { get; set; }
		public List<CustomPin> StopPins { get; set; }
		public CustomPin CurrPin { get; set; }

		public BusMap(MapSpan ms) : base(ms) {
			BusPins = new List<CustomPin> ();
			StopPins = new List<CustomPin> ();
		}
	}
}

