using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using GPSclocker.ViewModels;
using GPSclocker.Services;
using System.Diagnostics;

namespace GPSclocker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GpsNewItemPage : ContentPage
	{
        private Pin currentPin;
		public GpsNewItemPage ()
		{
			InitializeComponent ();
            // Инициализация карты (установите начальные координаты и уровень масштабирования)
            GetLocationAsync();
            BindingContext = new GpsNewItemViewModel();
        }
        private async void GetLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromKilometers(2)));
                }
                else
                {
                    // Если местоположение не доступно, вы можете предпринять необходимые действия
                    await DisplayAlert("Error", "Unable to get current location", "OK");
                }
            }
            catch (FeatureNotSupportedException)
            {
                // Местоположение не поддерживается на устройстве
                await DisplayAlert("Error", "Location not supported", "OK");
            }
            catch (PermissionException)
            {
                // Отсутствуют разрешения на местоположение
                await DisplayAlert("Error", "Location permission not granted", "OK");
            }
            catch (Exception ex)
            {
                // Другие ошибки при получении местоположения
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        public async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            // Удаление предыдущей метки
            if (currentPin != null)
                map.Pins.Remove(currentPin);

            // Добавление новой метки
            currentPin = new Pin
            {
                Type = PinType.Place,
                Position = new Position(e.Position.Latitude, e.Position.Longitude),
                Label = "Selected Location"
            };

            // Добавление метки в коллекцию вашей ViewModel для дальнейшей обработки
            if (BindingContext is GpsNewItemViewModel viewModel)
            {
                viewModel.Pins.Add(currentPin); // Используйте свойство Pins для добавления в коллекцию
            }

            map.Pins.Add(currentPin);
        }



    }
}