using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;
using GPSclocker.Models;
using System.Threading.Tasks;
using System.Linq;

namespace GPSclocker.ViewModels
{
    class GpsNewItemViewModel : GpsBaseViewMosel
    {
        private string description;
        private readonly GpsAlarmService alarmService = new GpsAlarmService();
        private Pin currentPin;
        private Xamarin.Forms.GoogleMaps.Map map; // Поле для хранения ссылки на карту
        private Page currentPage;
        private ObservableCollection<Pin> pins = new ObservableCollection<Pin>();
        
        public GpsNewItemViewModel(Xamarin.Forms.GoogleMaps.Map map, Page currentPage)
        {
            this.map = map;
            this.currentPage = currentPage;
            GetLocationAsync();
            SaveCommand = new Command(async () => await OnSave(), ValidateSave);
            CancelCommand = new Command(OnCancel);        
        }



        private bool ValidateSave()
        {
            // Добавьте здесь вашу логику валидации, если необходимо
            return true;
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public ObservableCollection<Xamarin.Forms.GoogleMaps.Pin> Pins
        {
            get => pins;
            set => SetProperty(ref pins, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void GetLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromKilometers(2)));
                    ShowCurrentLocationOnMap(location);
                }
                else
                {
                    await currentPage.DisplayAlert("Error", "Unable to get current location", "OK");
                }
            }
            catch (FeatureNotSupportedException)
            {
                await currentPage.DisplayAlert("Error", "Location not supported", "OK");
            }
            catch (PermissionException)
            {
                await currentPage.DisplayAlert("Error", "Location permission not granted", "OK");
            }
            catch (Exception ex)
            {
                await currentPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async void ShowCurrentLocationOnMap(Location location)
        {
            // Выполним геокодирование для получения адреса
            var addresses = await new Geocoder().GetAddressesForPositionAsync(new Position(location.Latitude, location.Longitude));

            string address = addresses.FirstOrDefault() ?? "Unknown Address";

            map.Circles.Add(new Circle
            {
                Center = new Position(location.Latitude, location.Longitude),
                Radius = Distance.FromMeters(5), // Радиус кружка в метрах
                StrokeColor = Color.Blue,
                FillColor = Color.FromRgba(0, 0, 255, 128)
            });
        }

        public async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            if (currentPin != null)
                map.Pins.Remove(currentPin);

            var position = new Position(e.Point.Latitude, e.Point.Longitude);

            // Выполним геокодирование для получения адреса
            var addresses = await new Geocoder().GetAddressesForPositionAsync(position);

            string address = addresses.FirstOrDefault() ?? "Unknown Address";

            currentPin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = "Selected Location",
                Address = address
            };


            Pins.Add(currentPin);

            map.Pins.Add(currentPin);
        }

        private async void OnCancel()
        {
            // Закрыть текущую страницу в стеке навигации
            await Shell.Current.GoToAsync("..");
        }

        private async Task OnSave()
        {
            if (Pins.Any())
            {
                // Получите последнюю метку из коллекции
                var lastPin = Pins.LastOrDefault();
                if (lastPin != null)
                {
                    GpsItem newItem = new GpsItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Description = Description,
                        Latitude = lastPin.Position.Latitude,
                        Longitude = lastPin.Position.Longitude,
                        Adress = lastPin.Address,
                        IsEnabled = true  
                    };

                    await DataStore.AddItemAsync(newItem);

                    // Добавление элемента в коллекцию меток
                    Pins.Add(new Pin
                    {
                        Type = PinType.Place,
                        Position = new Position(newItem.Latitude, newItem.Longitude),
                        Label = newItem.Description
                    });
                }

                // Закрыть текущую страницу в стеке навигации
                await Shell.Current.GoToAsync("..");
            }
        }

    }
}
