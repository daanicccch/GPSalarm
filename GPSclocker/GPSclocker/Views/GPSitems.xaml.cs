using GPSclocker.Models;
using GPSclocker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPSclocker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GPSitems : ContentPage
    {
        GpsItemsViewModel _viewModel;
        private AlarmService alarmService = new AlarmService();

        public GPSitems()
        {
            InitializeComponent();

            BindingContext = _viewModel = new GpsItemsViewModel();
        }

        private void OnAlarmToggle(object sender, ToggledEventArgs e)
        {
            var switchControl = (Switch)sender;
            var toggledAlarm = (Item)switchControl.BindingContext;

            if (switchControl.IsToggled)
            {
                alarmService.SetAlarmTimer(toggledAlarm);
            }
            else
            {
                alarmService.CancelAlarmTimer(toggledAlarm);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}