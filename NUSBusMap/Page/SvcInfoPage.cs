using System;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcInfoPage : ContentPage
	{
		public SvcInfoPage (string busSvcName)
		{
			Title = busSvcName + " Route Information";

			// show only for nus buses
			if (!BusHelper.IsPublic (busSvcName)) {
				Label header = new Label {
					Text = busSvcName,
					FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.Center
				};

				var view = new TableView () {
					Intent = TableIntent.Data
				};
				var root = new TableRoot ();
				var section = new TableSection ();

				foreach (string busStopCode in BusHelper.BusSvcs[busSvcName].stops) {
					section.Add (new BusStopCell (busStopCode));
				}

				root.Add (section);
				view.Root = root;

				Content = new StackLayout {
					Children = {
						header,
						view
					}
				};
			} else {
				Content = new Label { Text = "Page under construction" };
			}
        }
	}
}

