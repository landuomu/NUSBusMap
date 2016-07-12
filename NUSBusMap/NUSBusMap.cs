using System;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class App : Application
	{
		public App ()
		{
			// load bus data
			BusHelper.LoadBusData ();

			// Display the main page of your application
			MainPage = new NUSBusMap.MainPage ();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

