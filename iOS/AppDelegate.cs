using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using XLabs.Ioc; // Using for SimpleContainer
using XLabs.Platform.Services.Geolocation; // Using for Geolocation 
using XLabs.Platform.Device; // Using for Device

namespace NUSBusMap.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// init json stream loader
			JsonLoader.Loader = new StreamLoader ();

			// init xlabs for geolocator
			var container = new XLabs.Ioc.SimpleContainer(); // Create SimpleCOntainer
			container.Register<IDevice>(t => AppleDevice.CurrentDevice); // Register Device
			container.Register<IGeolocator, Geolocator>(); // Register Geolocator
			Resolver.SetResolver(container.GetResolver()); // Resolving the services

			// init xamarin forms and maps
			global::Xamarin.Forms.Forms.Init ();
			Xamarin.FormsMaps.Init();
			LoadApplication (new App ());
			return base.FinishedLaunching (app, options);
		}
	}
}

