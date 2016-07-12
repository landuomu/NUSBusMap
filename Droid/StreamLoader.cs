using System;
using System.IO;
using Android.Content;

namespace NUSBusMap
{
	public class StreamLoader : IStreamLoader
	{
		private readonly Context context;

		public StreamLoader (Context context)
		{
			this.context = context;
		}

		public Stream GetStreamFromFilename (string filename)
		{
			return context.Assets.Open (filename);
		}
	}
}

