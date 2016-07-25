using System;
using Xamarin.Forms;

namespace NUSBusMap
{
    public class BusStopCell : ViewCell
    {
        public BusStopCell(string busStopCode)
        {
			var grid = new Grid {
			    ColumnSpacing = 0,
			    RowSpacing = 0,
			    Padding = new Thickness(10),
			    RowDefinitions = { new RowDefinition { Height = new GridLength(10, GridUnitType.Star) } },
			    ColumnDefinitions = {
			        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
			        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
			    }
			};

            Label busStopName = new Label
            {
                Text = BusHelper.BusStops[busStopCode].name,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            Label roadName = new Label
            {
                Text = BusHelper.BusStops[busStopCode].road,
                TextColor = Color.Gray,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            grid.Children.Add(busStopName,0,0);
            grid.Children.Add(roadName,1,0);
            View = grid;
        }
    }
}