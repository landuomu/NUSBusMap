using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class PickerCell : ViewCell
	{
		public PickerCell (SettingsVarObj obj, EventHandler onSelectedIndexChanged)
		{
			// structure: row to be shown on Settings page
			// with display name of settings var and picker to edit var value

			var grid = new Grid {
			    ColumnSpacing = 0,
			    RowSpacing = 0,
			    Padding = new Thickness(10),
			    RowDefinitions = { new RowDefinition { Height = new GridLength(2, GridUnitType.Star) } },
			    ColumnDefinitions = {
			        new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) },
			        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
			    }
			};

			Label variableName = new Label {
				Text = obj.displayName,
				LineBreakMode = LineBreakMode.WordWrap,
				HorizontalTextAlignment = TextAlignment.Start,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			Picker variablePicker = new Picker {
				Title = obj.value.ToString(),
				StyleId = obj.name,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand
			};

			// add number of range into picker
			for (int i = obj.min; i <= obj.max; i += obj.step)
				variablePicker.Items.Add (i.ToString());
			variablePicker.SelectedIndexChanged += onSelectedIndexChanged;

			// add ui into grid
			grid.Children.Add (variableName, 0, 0);
			grid.Children.Add (variablePicker, 1, 0);
			View = grid;
		}
	}
}

