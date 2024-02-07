using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using GPSclocker.Models;
using System.Threading.Tasks;
using System.Linq;

namespace GPSclocker.ViewModels
{
    class GpsNewItemViewModel : GpsBaseViewMosel
    {
        private string description;
        private readonly GpsAlarmService alarmService = new GpsAlarmService();
        private ObservableCollection<Pin> pins = new ObservableCollection<Pin>();

        public GpsNewItemViewModel()
        {
            SaveCommand = new Command(async () => await OnSave(), ValidateSave);
            CancelCommand = new Command(OnCancel);

            //PropertyChanged +=
            //    (_, e) =>
            //    {

            //    };
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

        public ObservableCollection<Pin> Pins
        {
            get => pins;
            set => SetProperty(ref pins, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

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
