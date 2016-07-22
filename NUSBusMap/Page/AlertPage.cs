using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class AlertPage : ContentPage
	{
		public static ObservableCollection<BusStop> stops;
		public static List<string> enabledSvcs;
		public static List<string> enabledStops;
		private const int NUMBER_OF_COLS = 6;


		public class WrappedItemSelectionTemplate : ViewCell
        {
            public WrappedItemSelectionTemplate() : base ()
            {
                Label name = new Label();
                name.SetBinding(Label.TextProperty, new Binding("name"));

                Switch mainSwitch = new Switch();
                mainSwitch.SetBinding(Switch.IsToggledProperty, new Binding("alertEnabled"));
                mainSwitch.Toggled += (object sender, ToggledEventArgs e) => {
					var stop = (BusStop)(((Element)sender).Parent.BindingContext);
					if (stop == null) return; // stop removed from list

                	if (e.Value) {
                		if (!enabledStops.Contains(stop.busStopCode)) {
                			enabledStops.Add(stop.busStopCode);
                		}
                	} else {
						enabledStops.Remove(stop.busStopCode);
					}
                };

                RelativeLayout layout = new RelativeLayout();
                layout.Children.Add (name,
                    Constraint.Constant (5),
                    Constraint.Constant (5),
                    Constraint.RelativeToParent (p => p.Width - 60),
                    Constraint.RelativeToParent (p => p.Height - 10)
                );
                layout.Children.Add (mainSwitch,
                    Constraint.RelativeToParent (p => p.Width - 55),
                    Constraint.Constant (5),
                    Constraint.Constant (50),
                    Constraint.RelativeToParent (p => p.Height - 10)
                );
                View = layout;
            }
        }

		public AlertPage ()
		{
			// init lists
			stops = new ObservableCollection<BusStop> ();
			enabledSvcs = new List<string> ();
			enabledStops = new List<string> ();

			// create grid header
			var gridHeader = new Label { 
				Text = "Bus Services", 
				FontFamily = "Arial",
				FontAttributes = FontAttributes.Bold,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = 10
			};

			// create grid for bus services
			var grid = new Grid {
				ColumnSpacing = 20,
				RowSpacing = 10,
				RowDefinitions = {},
				ColumnDefinitions = {},
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center
			};

			// add columns
			double len = 10.0 / NUMBER_OF_COLS;
			for (int i=0;i<NUMBER_OF_COLS;i++) 
				grid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (len, GridUnitType.Star) });

			// add button for each bus service (incl public bus)
			int idx = 0;
			foreach (string routeName in BusHelper.BusSvcs.Keys.Union(BusHelper.PublicBusSvcStops.Keys)) {
				// add row if needed
				if (idx % NUMBER_OF_COLS == 0) 
					grid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (2, GridUnitType.Star) });

				Button routeBtn = new Button {
					Image = routeName + ".png",
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					StyleId = routeName,
					Opacity = 0.3,
					HeightRequest = 40,
					WidthRequest = 40,
					MinimumHeightRequest = 40,
					MinimumWidthRequest = 40
				};
				routeBtn.Clicked += OnClickRoute;
				grid.Children.Add (routeBtn, idx % NUMBER_OF_COLS, idx / NUMBER_OF_COLS);
				idx++;
			}

			// create list header
			var listHeader = new Label { 
				Text = "Bus Stops", 
				FontFamily = "Arial",
				FontAttributes = FontAttributes.Bold,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = 10
			};

			// create listview for bus stops
			var listView = new ListView ();
			listView.ItemsSource = stops;
			listView.ItemTemplate = new DataTemplate (typeof(WrappedItemSelectionTemplate));
			// disable default selection
			listView.ItemSelected += (sender, e) => {
			    ((ListView)sender).SelectedItem = null;
			};

			// create stack for all items
//			var stack = new StackLayout { Spacing = 10, Margin = 10 };
//			stack.Children.Add (gridHeader);
//			stack.Children.Add (grid);
//			stack.Children.Add (listHeader);
//			stack.Children.Add (listView);

			var mainGrid = new Grid {
				Margin = 10,
				RowDefinitions = {
					new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength (4, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength (4, GridUnitType.Star) }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (10, GridUnitType.Star) }
				},
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			mainGrid.Children.Add (gridHeader, 0, 0);
			mainGrid.Children.Add (grid, 0, 1);
			mainGrid.Children.Add (listHeader, 0, 2);
			mainGrid.Children.Add (listView, 0, 3);

			Icon = "AlertIcon.png";
			Title = "Alerts";
			Content = mainGrid;

			// check if need to display alert every interval
			CheckAlertTiming ();
		}

		private void OnClickRoute (object sender, EventArgs e)
		{
			var routeName = ((Button)sender).StyleId;
			// take the correct list for public/nus bus
			var busSvcStops = (BusHelper.IsPublic(routeName)) ? BusHelper.PublicBusSvcStops[routeName] : BusHelper.BusSvcs [routeName].stops;
			if (((Button)sender).Opacity.Equals(0.3)) {
				// activate bus service
				// add all stops from bus service into list if list does not contains bus stop
				((Button)sender).Opacity = 1;
				enabledSvcs.Add (routeName);
				foreach (string busStopCode in busSvcStops) 
					if (!stops.Contains(BusHelper.BusStops[busStopCode]))
						stops.Add (BusHelper.BusStops[busStopCode]);
			} else {
				// deactivate bus service
				// remove bus stop of bus service from list if no other enabled bus service shares the same bus stop
				// incl public bus
				((Button)sender).Opacity = 0.3;
				enabledSvcs.Remove (routeName);
				foreach (string busStopCode in busSvcStops) {
					var stop = BusHelper.BusStops [busStopCode];
					var otherEnabledSvcsInStop = enabledSvcs.Intersect (stop.services.Union(stop.publicServices)).ToList();
					if (otherEnabledSvcsInStop.Count == 0)
						stops.Remove (stop);
				}
			}
		}

		private async void CheckAlertTiming ()
		{
			while (true) {
				// check each stop and each svc that are enabled
				// incl public bus
				foreach (string busStopCode in enabledStops) {
					foreach (string routeName in enabledSvcs) {
						var stop = BusHelper.BusStops [busStopCode];
						if (stop.services.Union(stop.publicServices).Contains (routeName)) {
							// get arrival timing of svc in stop and display alert if below alert minutes
							var arrivalTimingStr = (stop.services.Contains(routeName)) ? 
													BusHelper.GetArrivalTiming (busStopCode, routeName) :
													await BusHelper.GetPublicBusesArrivalTiming (busStopCode, routeName);
							var nextTimingStr = Regex.Match (arrivalTimingStr, @"\d+").Value;
							if (!nextTimingStr.Equals (String.Empty)) {
								var nextTiming = Int32.Parse (nextTimingStr);
								if (nextTiming == 0) 
									await DisplayAlert ("Bus Alert", routeName + " is arriving " + stop.name + "!", "OK", "Cancel");
								else if (nextTiming <= SettingsVars.Variables ["ALERT_MINUTES"].value) 
									await DisplayAlert ("Bus Alert", routeName + " is arriving " + stop.name + " at " + nextTiming + " min.", "OK", "Cancel");
							} else if (arrivalTimingStr.Contains("Arr")) {
								await DisplayAlert ("Bus Alert", routeName + " is arriving " + stop.name + "!", "OK", "Cancel");
							}
						}
					}
				}

				// continue after interval
				await Task.Delay (TimeSpan.FromSeconds (SettingsVars.Variables ["REFRESH_ALERT_INTERVAL"].value));
			}
		}
	}
}

