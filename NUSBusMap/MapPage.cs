using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace NUSBusMap
{
	public class MapPage : ContentPage {
	    public MapPage() {
	    	// map with default centre at NUS
	        var map = new Map(
	            MapSpan.FromCenterAndRadius(
	                    new Position(1.2966,103.7764), Distance.FromKilometers(0.5))) {
	                IsShowingUser = true,
	                HeightRequest = 100,
	                WidthRequest = 960,
	                VerticalOptions = LayoutOptions.FillAndExpand
	            };
	        var stack = new StackLayout { Spacing = 0 };
	        stack.Children.Add(map);

	        // slider to change radius from 0.1 - 0.9 km
			var slider = new Slider (1, 9, 5);
			slider.ValueChanged += (sender, e) => {
			    var zoomLevel = e.NewValue; // between 1 and 9
			    var radius = 1.0 - (zoomLevel/10.0);
			    map.MoveToRegion(MapSpan.FromCenterAndRadius(
			    	map.VisibleRegion.Center, Distance.FromKilometers(radius)));
			};
			stack.Children.Add (slider);

	        Content = stack;
	    }
	}
}

