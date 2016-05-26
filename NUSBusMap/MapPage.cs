using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace NUSBusMap
{
	public class MapPage : ContentPage {
	    public MapPage() {
	        var map = new Map(
	            MapSpan.FromCenterAndRadius(
	                    new Position(1.2966,103.7764), Distance.FromKilometers(2.0))) {
	                IsShowingUser = true,
	                HeightRequest = 100,
	                WidthRequest = 960,
	                VerticalOptions = LayoutOptions.FillAndExpand
	            };
	        var stack = new StackLayout { Spacing = 0 };
	        stack.Children.Add(map);
	        Content = stack;
	    }
	}
}

