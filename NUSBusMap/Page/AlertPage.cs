using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class AlertPage : ContentPage
	{
		private ObservableCollection<BusStop> stops;
		private ListView listView;
		private List<string> enabledSvcs;

		public AlertPage ()
		{
			var gridHeader = new Label { 
				Text = "Bus Services", 
				FontFamily = "Arial",
				FontAttributes = FontAttributes.Bold,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = 10
			};

			var grid = new Grid {
				ColumnSpacing = 10,
				RowSpacing = 10,
				Padding = new Thickness (10),
				RowDefinitions = { 
					new RowDefinition { Height = new GridLength (3, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength (3, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength (3, GridUnitType.Star) }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) }
				},
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			var listHeader = new Label { 
				Text = "Bus Stops", 
				FontFamily = "Arial",
				FontAttributes = FontAttributes.Bold,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = 10
			};

			stops = new ObservableCollection<BusStop> ();
			enabledSvcs = new List<string> ();
			listView = new ListView ();
			listView.ItemsSource = stops;

			int idx = 0;
			foreach (string routeName in BusHelper.BusSvcs.Keys) {
				Button routeBtn = new Button {
					Image = routeName + ".png",
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					StyleId = routeName,
					Opacity = 0.3
				};
				routeBtn.Clicked += OnClickRoute;
				grid.Children.Add (routeBtn, idx % 5, idx / 5);
				idx++;
			}

			var stack = new StackLayout { Spacing = 10, Margin = 10 };
			stack.Children.Add (gridHeader);
			stack.Children.Add (grid);
			stack.Children.Add (listHeader);
			stack.Children.Add (listView);

			Icon = "AlertIcon.png";
			Title = "Alerts";
			Content = stack;
		}

		private void OnClickRoute (object sender, EventArgs e)
		{
			var routeName = ((Button)sender).StyleId;
			// TODO: Add bus stops the route pass by into the bus stop list
			if (((Button)sender).Opacity.Equals(0.3)) {
				// activate bus service
				// add all stops from bus service into list if list does not contains bus stop
				((Button)sender).Opacity = 1;
				enabledSvcs.Add (routeName);
				foreach (string busStopCode in BusHelper.BusSvcs[routeName].stops) 
					if (!stops.Contains(BusHelper.BusStops[busStopCode]))
						stops.Add (BusHelper.BusStops[busStopCode]);
			} else {
				// deactivate bus service
				// remove bus stop of bus service from list if no other enabled bus service shares the same bus stop
				((Button)sender).Opacity = 0.3;
				enabledSvcs.Remove (routeName);
				foreach (string busStopCode in BusHelper.BusSvcs[routeName].stops) {
					var stop = BusHelper.BusStops [busStopCode];
					var otherEnabledSvcsInStop = enabledSvcs.Intersect (stop.services).ToList();
					if (otherEnabledSvcsInStop.Count == 0)
						stops.Remove (stop);
				}
			}
		}
	}
}

