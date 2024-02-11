using GPSclocker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace GPSclocker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GpsItemDetailPage : ContentPage
	{
        public GpsItemDetailPage()
        {
            InitializeComponent();
            var viewModel = new GpsItemDetailViewModel(map, this);
            BindingContext = viewModel;
            map.MapClicked += viewModel.Map_MapClicked;
        }
    }
}