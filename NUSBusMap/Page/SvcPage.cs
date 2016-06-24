using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcPage : ContentPage
	{
		public SvcPage ()
		{
			// create table view
			var view = new TableView () { Intent = TableIntent.Data };
			var root = new TableRoot ();

			// add header row
			var headerSection = new TableSection ();
			var headerCell = new ViewCell ();
			var grid = new Grid {
			    ColumnSpacing = 0,
			    RowSpacing = 0,
			    Padding = new Thickness(10),
			    RowDefinitions = { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } },
			    ColumnDefinitions = {
			        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
			        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
			        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
			    },
			    HorizontalOptions = LayoutOptions.Center
			};
			grid.Children.Add (new Label { 
				Text = "Route Name", 
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			}, 0, 0);
			grid.Children.Add (new Label { 
				Text = "Show on Map", 
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
			}, 1, 0);
			grid.Children.Add (new Label { 
				Text = "Route Info", 
				HorizontalOptions = LayoutOptions.Center, 
				VerticalOptions = LayoutOptions.Center
			}, 2, 0);
			headerCell.View = grid;
			headerSection.Add (headerCell);
			root.Add (headerSection);

			var section = new TableSection ();
			// add each bus service switch and info button
			foreach (string routeName in BusHelper.BusSvcs.Keys) {
				section.Add(new SvcCell (routeName, onToggleSvc, onClickInfo));
			}
			root.Add (section);
			view.Root = root;

			Icon = "BusTabIcon.png";
			Title = "Bus Services";
			Content = view;
		}

		private void onToggleSvc (object sender, ToggledEventArgs e)
		{
			// show/hide buses of routeName (sender.StyleId) on map
			BusHelper.BusSvcs[((Switch)sender).StyleId].showOnMap = e.Value;
		}

		private async void onClickInfo (object sender, EventArgs e)
		{
			// open bus route info for routeName (sender.StyleId)
			await Navigation.PushAsync (new SvcInfoPage (((Button)sender).StyleId));
		}
	}
}

