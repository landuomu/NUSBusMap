using System;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcPage : ContentPage
	{
		public SvcPage ()
		{
			Icon = "BusTabIcon.png";
			Title = "Bus Services";
			Content = new Label {
				Text = "Service Page",
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};
		}
	}
}

