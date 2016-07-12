using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public static class SettingsVars
	{
		// dictionary to hold all variables which user can set in Settings Page
		public static Dictionary<string,double> Variables = new Dictionary<string, double> () {
			{"MEAN_MAP_RADIUS", 0.5},
			{"REFRESH_BUS_INTERVAL", 3},
			{"REFRESH_STOP_INTERVAL", 30},
			{"REFRESH_ALERT_INTERVAL", 60},
			{"MARGIN_OF_ERROR", 10.0},
			{"ALERT_MINUTES", 3}
		};
//		public static double MEAN_MAP_RADIUS = 0.5; // in km
//		public static int REFRESH_BUS_INTERVAL = 3; // in s
//		public static int REFRESH_STOP_INTERVAL = 30; // in s
//		public static int REFRESH_ALERT_INTERVAL = 10; // in s
//		public static double MARGIN_OF_ERROR = 10.0; // in m, max distance allowed between bus and stop to be considered reached stop
//		public static int ALERT_MINUTES = 3; // in min
	}
}

