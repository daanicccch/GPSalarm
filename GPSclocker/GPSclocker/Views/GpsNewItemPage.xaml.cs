using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using GPSclocker.ViewModels;
using GPSclocker.Services;
using System.Diagnostics;
using System;
using System.Linq;

namespace GPSclocker.Views
{
    public partial class GpsNewItemPage : ContentPage
    {

        public GpsNewItemPage()
        {
            InitializeComponent();
            var viewModel = new GpsNewItemViewModel(map, this);
            BindingContext = viewModel;
            map.MapClicked += viewModel.Map_MapClicked;
        }

        
    }
}
