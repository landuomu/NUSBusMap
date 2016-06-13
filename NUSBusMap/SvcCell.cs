using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcCell : ViewCell
	{
		private BusSvc busSvc;

		public SvcCell (BusSvc bs, EventHandler onToggleSvc, EventHandler onClickInfo)
		{
			busSvc = bs;

			var stack = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5)
			};

			Label serviceLabel = new Label {
				Text = busSvc.routeName,
				TextColor = Color.Blue,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center
			};

			Switch switcher = new Switch {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			switcher.Toggled += onToggleSvc;

			Button svcInfoBtn = new Button {
				Image = "Info.png",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			svcInfoBtn.Clicked += onClickInfo;

			stack.Children.Add (serviceLabel);
			stack.Children.Add (switcher);
			stack.Children.Add (svcInfoBtn);
			View = stack;
		}
	}
}

