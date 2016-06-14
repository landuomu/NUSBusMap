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
			// test
		}

		private void onToggleSvc (object sender, ToggledEventArgs e)
		{
			// TODO: show/hide buses of routeName on map
		}

		private async void onClickInfo (object sender, EventArgs e)
		{
			await Navigation.PushAsync (new SvcInfoPage ());
		}
	}
}

