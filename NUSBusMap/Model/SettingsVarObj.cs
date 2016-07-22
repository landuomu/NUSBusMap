using System;

namespace NUSBusMap
{
	public class SettingsVarObj
	{
		public string name { get; set; } // primary key
		// variables to display ui
		public SettingsVars.Section section { get; set; }
		public string displayName { get; set; }
		// variables to set up picker ui
		public int value { get; set; }
		public int min { get; set; }
		public int max { get; set; }
		public int step { get; set; }
	}
}

