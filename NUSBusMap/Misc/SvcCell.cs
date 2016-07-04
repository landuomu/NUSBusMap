using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcCell : ViewCell
	{
		public SvcCell (string svc, EventHandler<ToggledEventArgs> onToggleSvc, EventHandler onClickInfo)
		{
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

			Image serviceImage = new Image {
				Source = svc + ".png",
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center
			};

			Switch switcher = new Switch {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start,
				StyleId = svc
			};
			switcher.Toggled += onToggleSvc;

			Button svcInfoBtn = new Button {
				Image = "Info.png",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				StyleId = svc
			};
			svcInfoBtn.Clicked += onClickInfo;

			grid.Children.Add (serviceImage, 0, 0);
			grid.Children.Add (switcher, 1, 0);
			grid.Children.Add (svcInfoBtn, 2, 0);
			View = grid;
		}
	}
}

