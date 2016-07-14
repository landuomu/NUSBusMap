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

		// add subsequently from PublicBusSvc
		public string ServiceNo { get; set; }
		public string OriginatingID { get; set; }
		public string TerminatingID { get; set; }

		// comparison method
		public bool IsSameBus(PublicBusOnRoad bus) {
			return this.Latitude.Equals(bus.Latitude) && this.Longitude.Equals(bus.Longitude);
		}
	}
}

