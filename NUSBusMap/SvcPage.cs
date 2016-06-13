using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcPage : ContentPage
	{
		public List<BusSvc> busSvcs;

		public SvcPage ()
		{
			// load bus services from json
			busSvcs = (List<BusSvc>) JsonLoader.LoadSvcs ();

			// create table view
			var view = new TableView () { Intent = TableIntent.Settings };
			var root = new TableRoot ();
			var section = new TableSection ();
			// add each bus service switch and info button
			foreach (BusSvc busSvc in busSvcs) {
				section.Add(new SvcCell (busSvc, onToggleSvc, onClickInfo));
			}
			root.Add (section);
			view.Root = root;

			Icon = "BusTabIcon.png";
			Title = "Bus Services";
			Content = view;
		}

		private void onToggleSvc (object sender, EventArgs e)
		{
			// TODO: show/hide buses of routeName on map
		}

		private void onClickInfo (object sender, EventArgs e)
		{
			// TODO: get list of bus stops of routeName (display service info page)
			// await Navigation.PushAsync (new SvcInfoPage ());
			System.Diagnostics.Debug.WriteLine ("Sender: {0}", sender);
		}
	}
}

