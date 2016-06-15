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
			var view = new TableView () { Intent = TableIntent.Settings };
			var root = new TableRoot ();
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

