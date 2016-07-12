using System;

using Xamarin.Forms;

namespace NUSBusMap
{
	public class SvcInfoPage : ContentPage
	{
		public SvcInfoPage (string routeName)
		{
			Title = routeName + " Route Information";
			Content = new Label { Text = "Page under construction" };
		}
	}
}

