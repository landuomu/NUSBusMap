using System;

namespace NUSBusMap
{
	public class PublicBusOnRoad
	{
		// model based on LTA DataMall API
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
		// same bus: same service no, same originating/terminating id, almost same location (within offset)
		public bool IsSameBus(PublicBusOnRoad bus, double offset) {
			return (this.ServiceNo.Equals(bus.ServiceNo) && this.OriginatingID.Equals(bus.OriginatingID) && 
				this.TerminatingID.Equals(bus.TerminatingID)) &&
				(distance(this.Latitude.GetValueOrDefault(),this.Longitude.GetValueOrDefault(),bus.Latitude.GetValueOrDefault(),bus.Longitude.GetValueOrDefault()) < offset);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//:::                                                                         :::
		//:::  This routine calculates the distance between two points (given the     :::
		//:::  latitude/longitude of those points). It is being used to calculate     :::
		//:::  the distance between two locations using GeoDataSource(TM) products    :::
		//:::                                                                         :::
		//:::  Definitions:                                                           :::
		//:::    South latitudes are negative, east longitudes are positive           :::
		//:::                                                                         :::
		//:::  Passed to function:                                                    :::
		//:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
		//:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
		//:::    unit = the unit you desire for results                               :::
		//:::           where: 'M' is statute miles (default)                         :::
		//:::                  'K' is kilometers                                      :::
		//:::                  'N' is nautical miles                                  :::
		//:::                                                                         :::
		//:::  Worldwide cities and other features databases with latitude longitude  :::
		//:::  are available at http://www.geodatasource.com                          :::
		//:::                                                                         :::
		//:::  For enquiries, please contact sales@geodatasource.com                  :::
		//:::                                                                         :::
		//:::  Official Web site: http://www.geodatasource.com                        :::
		//:::                                                                         :::
		//:::           GeoDataSource.com (C) All Rights Reserved 2015                :::
		//:::                                                                         :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		private double distance(double lat1, double lon1, double lat2, double lon2) {
		  double theta = lon1 - lon2;
		  double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
		  dist = Math.Acos(dist);
		  dist = rad2deg(dist);
		  dist = dist * 60 * 1.1515;
		  dist = dist * 1.609344;
		  return (dist * 1000);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts decimal degrees to radians             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private double deg2rad(double deg) {
		  return (deg * Math.PI / 180.0);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts radians to decimal degrees             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private double rad2deg(double rad) {
		  return (rad / Math.PI * 180.0);
		}
	}
}

