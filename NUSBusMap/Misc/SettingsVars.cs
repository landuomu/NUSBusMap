using System;
using System.Collections.Generic;

namespace NUSBusMap
{
	public static class SettingsVars
	{
		// dictionary to hold all variables which user can set in Settings Page
		public enum Section { MAP, ALERTS, OTHERS };
		public static Dictionary<string,SettingsVarObj> Variables = new Dictionary<string, SettingsVarObj> () {
			{"REFRESH_BUS_INTERVAL", new SettingsVarObj () { 
				name = "REFRESH_BUS_INTERVAL",
				section = Section.MAP, 
				displayName = "Refresh bus location every (s)", 
				value = 5,
				min = 3,
				max = 15, 
				step = 1
			} },
			{"REFRESH_PUBLIC_BUS_INTERVAL", new SettingsVarObj () { 
				name = "REFRESH_PUBLIC_BUS_INTERVAL",
				section = Section.MAP, 
				displayName = "Refresh public bus location every (s)", 
				value = 15,
				min = 5,
				max = 30, 
				step = 5
			} },
			{"REFRESH_STOP_INTERVAL", new SettingsVarObj () { 
				name = "REFRESH_STOP_INTERVAL",
				section = Section.MAP, 
				displayName = "Refresh bus stop timing every (s)", 
				value = 30,
				min = 15,
				max = 60,
				step = 5
			} },
			{"REFRESH_ALERT_INTERVAL", new SettingsVarObj () { 
				name = "REFRESH_ALERT_INTERVAL",
				section = Section.ALERTS, 
				displayName = "Check alerts every (s)", 
				value = 60,
				min = 30,
				max = 180,
				step = 10
			} },
			{"ALERT_MINUTES", new SettingsVarObj () { 
				name = "ALERT_MINUTES",
				section = Section.ALERTS, 
				displayName = "Alert when bus arriving in (min)",
				value = 3,
				min = 1,
				max = 10,
				step = 1
			} }
		};
	}
}

