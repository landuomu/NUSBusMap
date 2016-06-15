using System;
using System.Drawing;
using System.Collections.Generic;
using CoreGraphics;
using NUSBusMap;
using NUSBusMap.iOS;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer (typeof(BusMap), typeof(BusMapRenderer))]
namespace NUSBusMap.iOS
{
	public class BusMapRenderer : MapRenderer
	{
		UIView customPinView;
		List<CustomPin> busPins;
		List<CustomPin> stopPins;

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null) {
				var nativeMap = Control as MKMapView;
				nativeMap.GetViewForAnnotation = null;
				// nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
				nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
				nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
			}

			if (e.NewElement != null) {
				var formsMap = (BusMap)e.NewElement;
				var nativeMap = Control as MKMapView;
				busPins = formsMap.BusPins;
				stopPins = formsMap.StopPins;

				nativeMap.GetViewForAnnotation = GetViewForAnnotation;
				// nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
				nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
				nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
			}
		}

		MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation is MKUserLocation)
				return null;
			
			var anno = annotation as MKPointAnnotation;
			var customPin = GetCustomPin (anno);
			if (customPin == null) {
				throw new Exception ("Custom pin not found");
			}

			annotationView = mapView.DequeueReusableAnnotation (customPin.Id);
			if (annotationView == null) {
				annotationView = new MKAnnotationView (annotation, customPin.Id);
				annotationView.Image = UIImage.FromFile (customPin.Url).Scale(new SizeF() { Height=20, Width=20 });
			}
			annotationView.CanShowCallout = true;

			return annotationView;
		}

//		void OnCalloutAccessoryControlTapped (object sender, MKMapViewAccessoryTappedEventArgs e)
//		{
//			var customView = e.View as CustomMKPinAnnotationView;
//			if (!string.IsNullOrWhiteSpace (customView.Url)) {
//				UIApplication.SharedApplication.OpenUrl (new Foundation.NSUrl (customView.Url));
//			}
//		}

		void OnDidSelectAnnotationView (object sender, MKAnnotationViewEventArgs e)
		{
			var customView = e.View as CustomMKPinAnnotationView;
			customPinView = new UIView ();

			customPinView.Frame = new CGRect (0, 0, 200, 84);
//			var image = new UIImageView (new CGRect (0, 0, 200, 84));
//			image.Image = UIImage.FromFile (customView.Url);
//			customPinView.AddSubview (image);
			customPinView.Center = new CGPoint (0, -(e.View.Frame.Height + 75));
			e.View.AddSubview (customPinView);
		}

		void OnDidDeselectAnnotationView (object sender, MKAnnotationViewEventArgs e)
		{
			if (!e.View.Selected) {
				customPinView.RemoveFromSuperview ();
				customPinView.Dispose ();
				customPinView = null;
			}
		}

		CustomPin GetCustomPin (MKPointAnnotation annotation)
		{
			var position = new Position (annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
			foreach (var pin in busPins) {
				if (pin.Pin.Position == position) {
					return pin;
				}
			}
			foreach (var pin in stopPins) {
				if (pin.Pin.Position == position) {
					return pin;
				}
			}
			return null;
		}
	}
}
