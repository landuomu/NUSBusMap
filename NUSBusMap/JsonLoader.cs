using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace NUSBusMap
{
	public static class JsonLoader
	{
		const string stopsFilename = "BusStops.json";
		const string svcsFilename = "BusSvcs.json";

		public static IStreamLoader Loader { get; set;}

		public static  IEnumerable<BusStop> LoadStops()
		{
			using (var reader = new StreamReader(OpenData(stopsFilename))) {
				return JsonConvert.DeserializeObject<List<BusStop>>( reader.ReadToEnd());
			}
		}

		public static IEnumerable<BusSvc> LoadSvcs()
		{
			using (var reader = new StreamReader(OpenData(svcsFilename))) {
				return JsonConvert.DeserializeObject<List<BusSvc>>(reader.ReadToEnd());
			}
		}

		private static Stream OpenData(string filename)
		{
			if (Loader == null)
				throw new Exception ("Must set platform before calling Load.");
			
			return Loader.GetStreamFromFilename(filename);
		}
	}
}

