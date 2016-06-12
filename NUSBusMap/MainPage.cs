using System;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class MainPage : TabbedPage
	{
		public MainPage ()
		{
			Children.Add (new MapPage ());
			Children.Add (new SvcPage ());
		}
	}
}

