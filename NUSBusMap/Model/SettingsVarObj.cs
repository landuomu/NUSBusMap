using System;

namespace NUSBusMap
{
	public class SettingsVarObj
	{
		public string name { get; set; }
		public SettingsVars.Section section { get; set; }
		public string displayName { get; set; }
		public int value { get; set; }
		public int min { get; set; }
		public int max { get; set; }
		public int step { get; set; }
	}
}

