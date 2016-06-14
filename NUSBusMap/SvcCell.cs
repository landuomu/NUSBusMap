using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcCell : ViewCell
	{
		public SvcCell (string svc, EventHandler<ToggledEventArgs> onToggleSvc, EventHandler onClickInfo)
		{
			var stack = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(10)
			};

			Label serviceLabel = new Label {
				Text = svc,
				TextColor = Color.Blue,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center
			};

			Switch switcher = new Switch {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start
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

