using System;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class AlertPage : ContentPage
	{
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

			Icon = "AlertIcon.png";
			Title = "Alerts";
			Content = stack;
		}

		private void OnClickRoute (object sender, EventArgs e)
		{
			// TODO: Add bus stops the route pass by into the bus stop list
			if (((Button)sender).Opacity.Equals(0.3)) {
				// activate bus service
				((Button)sender).Opacity = 1;
			} else {
				// deactivate bus service
				((Button)sender).Opacity = 0.3;
			}
		}
	}
}

