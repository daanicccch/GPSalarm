using GPSclocker.Models;
using GPSclocker.ViewModels;
using GPSclocker.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPSclocker.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;
        private AlarmService alarmService = new AlarmService();

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
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
