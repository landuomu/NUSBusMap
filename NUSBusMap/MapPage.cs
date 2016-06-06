using System;
using System.Collections.Generic;
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

	        // pins for bus stops
			// var position = new Position(37,-122); // Latitude, Longitude
			// testing list of bus stops
			List<Position> busStops = new List<Position>() {
				new Position (1.29630305719228, 103.78341318580553), // NUH
				new Position (1.293383, 103.784394) // Kent Ridge MRT Station
			};
			// busStops.Add(new Position (1.29630305719228, 103.78341318580553));
			foreach (Position position in busStops) {
				// var position = new Position (1.29630305719228, 103.78341318580553);
				var pin = new Pin {
				            Type = PinType.Place,
				            Position = position,
				            Label = "Bus stop name - Bus stop code",
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

	        Content = stack;
	    }
	}
}

