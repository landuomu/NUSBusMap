using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XLabs.Platform.Device;
using XLabs.Platform;
using XLabs.Ioc;
using XLabs.Platform.Services.Geolocation;

namespace NUSBusMap
{
	public class MapPage : ContentPage {
		private Map map;
		private const double DEFAULT_RADIUS = 0.5;

		// for pinching gesture
		// currently Xamarin.Forms only supports tap gesture
		// KIV for platform-specific implementation
//		private double startScale, currentScale;
//		private double xOffset = 0;
//		private double yOffset = 0;

	    public MapPage() {
	    	// map with default centre at NUS
			var NUSCenter = new Xamarin.Forms.Maps.Position (1.2966, 103.7764);
	        map = new Map(
	            MapSpan.FromCenterAndRadius(NUSCenter, Distance.FromKilometers(DEFAULT_RADIUS))) {
	                IsShowingUser = true,
	                HeightRequest = 100,
	                WidthRequest = 960,
	                VerticalOptions = LayoutOptions.FillAndExpand,
	                HasZoomEnabled = true,
	                HasScrollEnabled = true
	            };

			// add pinch gesture to zoom in/out map
			// currently Xamarin.Forms only supports tap gesture
			// KIV for platform-specific implementation
			// var pinchGesture = new PinchGestureRecognizer ();
		    // pinchGesture.PinchUpdated += OnPinchUpdated;
		    // map.GestureRecognizers.Add (pinchGesture);

	        // shift to current location if possible (activate only for device testing)
			// ShiftToCurrentLocation ();

	        // add pins for each bus stops
	        foreach (BusStop busStop in BusHelper.BusStops) {
				var pin = new Pin {
				            Type = PinType.Place,
							Position = new Xamarin.Forms.Maps.Position(busStop.latitude, busStop.longitude),
				            Label = busStop.name + " - " + busStop.busStopCode,
							Address = "Bus service - Bus arrival timings"
				        };
				map.Pins.Add(pin);
			}

	        // slider to change radius from 0.1 - 0.9 km
			var slider = new Slider (1, 9, 5);
			slider.ValueChanged += (sender, e) => {
			    var zoomLevel = e.NewValue; // between 1 and 9
			    var radius = 1.0 - (zoomLevel/10.0);
			    map.MoveToRegion(MapSpan.FromCenterAndRadius(
			    	map.VisibleRegion.Center, Distance.FromKilometers(radius)));
			};

			// add map and slider to stack layout
	        var stack = new StackLayout { Spacing = 0 };
	        stack.Children.Add(map);
			stack.Children.Add(slider);

			Icon = "MapTabIcon.png";
			Title = "Map";
	        Content = stack;

	    }

		private void ShiftToCurrentLocation () {
			var geolocation = GetCurrentPosition ().ContinueWith(t => {
	            if (t.IsFaulted)
	            {
	                System.Diagnostics.Debug.WriteLine("Error : {0}", ((GeolocationException)t.Exception.InnerException).Error.ToString());
	            }
	            else if (t.IsCanceled)
	            {
	                System.Diagnostics.Debug.WriteLine("Error : The geolocation has got canceled !");
	            }
	            else
	            {
	                var currentLocation = new Xamarin.Forms.Maps.Position (t.Result.Latitude, t.Result.Longitude);

	                map.MoveToRegion(MapSpan.FromCenterAndRadius(currentLocation, Distance.FromKilometers(DEFAULT_RADIUS)));
	            }
	        }, TaskScheduler.FromCurrentSynchronizationContext ());
	    }

		public async Task<XLabs.Platform.Services.Geolocation.Position> GetCurrentPosition() {
			IGeolocator geolocator = Resolver.Resolve<IGeolocator>();

			XLabs.Platform.Services.Geolocation.Position result = null;

	        if (geolocator.IsGeolocationEnabled) {
	            try {
	                if (!geolocator.IsListening)
	                    geolocator.StartListening(1000, 1000);

	                var task = await geolocator.GetPositionAsync(10000);

	                result = task;

	                System.Diagnostics.Debug.WriteLine("[GetPosition] Lat. : {0} / Long. : {1}", result.Latitude.ToString("N4"), result.Longitude.ToString("N4"));
	            }
	            catch (Exception e) {
	                System.Diagnostics.Debug.WriteLine ("Error : {0}", e);
	            }
	        }

	        return result;
	    }

		// currently Xamarin.Forms only supports tap gesture
		// KIV for platform-specific implementation

//		void OnPinchUpdated (object sender, PinchGestureUpdatedEventArgs e)
//		{
//		  if (e.Status == GestureStatus.Started) {
//		    // Store the current scale factor applied to the wrapped user interface element,
//		    // and zero the components for the center point of the translate transform.
//		    startScale = Content.Scale;
//		    Content.AnchorX = 0;
//		    Content.AnchorY = 0;
//		  }
//		  if (e.Status == GestureStatus.Running) {
//		    // Calculate the scale factor to be applied.
//		    currentScale += (e.Scale - 1) * startScale;
//		    currentScale = Math.Max (1, currentScale);
//
//		    // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
//		    // so get the X pixel coordinate.
//		    double renderedX = Content.X + xOffset;
//		    double deltaX = renderedX / Width;
//		    double deltaWidth = Width / (Content.Width * startScale);
//		    double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;
//
//		    // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
//		    // so get the Y pixel coordinate.
//		    double renderedY = Content.Y + yOffset;
//		    double deltaY = renderedY / Height;
//		    double deltaHeight = Height / (Content.Height * startScale);
//		    double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;
//
//		    // Calculate the transformed element pixel coordinates.
//		    double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
//		    double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);
//
//		    // Apply translation based on the change in origin.
//		    Content.TranslationX = Clamp (targetX, -Content.Width * (currentScale - 1), 0);
//		    Content.TranslationY = Clamp (targetY, -Content.Height * (currentScale - 1), 0);
//
//		    // Apply scale factor.
//		    Content.Scale = currentScale;
//		  }
//		  if (e.Status == GestureStatus.Completed) {
//		    // Store the translation delta's of the wrapped user interface element.
//		    xOffset = Content.TranslationX;
//		    yOffset = Content.TranslationY;
//		  }
//		}
//
//		private double Clamp(double value, double max, double min) {
//			if (value > max)
//				return max;
//			else if (value < min)
//				return min;
//			else
//				return value;
//		}
	}
}

