using System;

namespace NUSBusMap
{
	public class PublicBusSvc
	{
		public string ServiceNo { get; set; }
		public string Status { get; set; }
		public string Operator { get; set; }
		public string OriginatingID { get; set; }
		public string TerminatingID { get; set; }
		public PublicBusOnRoad NextBus { get; set; }
		public PublicBusOnRoad SubsequentBus { get; set; }
		public PublicBusOnRoad SubsequentBus3 { get; set; }
	}
}

