using System;

namespace NUSBusMap
{
	public class PublicBusOnRoad
	{
		public DateTime? EstimatedArrival { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public int? VisitNumber { get; set; }
		public string Load { get; set; }
		public string Feature { get; set; }
	}
}

