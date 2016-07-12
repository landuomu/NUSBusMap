using System;
using System.IO;

namespace NUSBusMap
{
	class StreamLoader : IStreamLoader
	{
		public StreamLoader ()
		{

		}

		public Stream GetStreamFromFilename(string filename) 
		{
			return File.OpenRead(filename);
		}
	}
}

