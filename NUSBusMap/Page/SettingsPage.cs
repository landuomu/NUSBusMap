using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NUSBusMap
{
	public class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			// create table view
			var view = new TableView () { Intent = TableIntent.Settings };
			var root = new TableRoot ();

			var mapSection = new TableSection ("Map Display");
			foreach(KeyValuePair<string,SettingsVarObj> variable in 
					SettingsVars.Variables.Where(v => v.Value.section.Equals(SettingsVars.Section.MAP))) {
				mapSection.Add (new PickerCell(variable.Value,OnSelectedIndexChanged));
			}
			root.Add (mapSection);
			var alertSection = new TableSection ("Alerts");
			foreach(KeyValuePair<string,SettingsVarObj> variable in 
					SettingsVars.Variables.Where(v => v.Value.section.Equals(SettingsVars.Section.ALERTS))) {
				alertSection.Add (new PickerCell(variable.Value,OnSelectedIndexChanged));
			}
			root.Add (alertSection);
			view.Root = root;

			// create stack for all items
			var stack = new StackLayout { Spacing = 0, Margin = 0 };
			stack.Children.Add (view);

			Icon = "SettingsTabIcon.png";
			Title = "Settings";
			Padding = new Thickness (0, Device.OnPlatform (20, 0, 0), 0, 0);
			Content = stack;
		}

		private void OnSelectedIndexChanged (object sender, EventArgs e)
		{
			// store new value into variables
			var picker = (Picker)sender;
			if (picker.SelectedIndex != -1) {
				SettingsVars.Variables [picker.StyleId].value = Convert.ToInt32(picker.Items[picker.SelectedIndex]);
			}
		}
	}
}

