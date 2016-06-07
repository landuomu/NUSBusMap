using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

// New Xlabs
using XLabs.Ioc; // Using for SimpleContainer
using XLabs.Platform.Services.Geolocation; // Using for Geolocation 
using XLabs.Platform.Device; // Using for Display
// End new Xlabs

namespace NUSBusMap.Droid
{
	[Activity (Label = "NUSBusMap.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// init json stream loader
			JsonLoader.Loader = new StreamLoader (this);

			// init Xlabs for geolocator
			var container = new SimpleContainer();
			container.Register<IDevice> (t => AndroidDevice.CurrentDevice); 
			container.Register<IGeolocator, Geolocator>(); 
			Resolver.SetResolver(container.GetResolver()); // Resolving the services

			// init xamarin forms and maps
			global::Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init (this, bundle);
			LoadApplication (new App ());
		}
	}
}

