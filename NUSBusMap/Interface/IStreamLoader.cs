using System;

namespace NUSBusMap
{
	public interface IStreamLoader
	{
		System.IO.Stream GetStreamFromFilename(string filename);
	}
}

