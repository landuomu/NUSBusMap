using System;
using Xamarin.Forms;

namespace NUSBusMap
{
	public class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			// create table view
			var view = new TableView () { Intent = TableIntent.Data };
			var root = new TableRoot ();

			var mapSection = new TableSection ("Map Display");
			root.Add (mapSection);
			var alertSection = new TableSection ("Alerts");
			root.Add (alertSection);
			view.Root = root;

			// create stack for all items
			var stack = new StackLayout { Spacing = 10, Margin = 10 };


			Icon = "SettingsTabIcon.png";
			Title = "Settings";
			Content = stack;
		}
	}
}

