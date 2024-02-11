using GPSclocker.Models;
using GPSclocker.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.GoogleMaps;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

namespace GPSclocker.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class GpsItemDetailViewModel : GpsBaseViewMosel
    {
        private string itemId;
        private string adress;
        private string description;
        private Pin currentPin;
        private Xamarin.Forms.GoogleMaps.Map map; // Поле для хранения ссылки на карту
        private Page currentPage;
        private ObservableCollection<Pin> pins = new ObservableCollection<Pin>();


        public GpsItemDetailViewModel(Xamarin.Forms.GoogleMaps.Map map, Page currentPage)
        {   
            this.map = map;
            this.currentPage = currentPage;
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
        }

        public ObservableCollection<Xamarin.Forms.GoogleMaps.Pin> Pins
        {
            get => pins;
            set => SetProperty(ref pins, value);
        }
        public async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            map.Pins.Clear();
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
        private async void GetLocationAsync(Position position)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                });

                if (location != null)
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(2)));
                    
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

        private void ShowCurrentLocationOnMap(Position position)
        {
            map.Pins.Clear();

            var pin = new Pin
            {
                Position = position,
                Label = "Selected Location",
                Address = Description 
            };

            map.Pins.Add(pin);
        }
        
        public string Id { get; set; }
        private bool ValidateSave()
        {
            return true;
        }

        public string Adress
        {
            get => adress;
            set => SetProperty(ref adress, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {         
            var lastPin = Pins.LastOrDefault();
            DataStore.DeleteItemAsync(ItemId);
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

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Id = item.Id;
                Adress = item.Adress;
                Description = item.Description;
                var position = new Position(item.Latitude, item.Longitude);
                GetLocationAsync(position);
                ShowCurrentLocationOnMap(position);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}