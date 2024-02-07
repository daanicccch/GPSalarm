using GPSclocker.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GPSclocker.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private TimeSpan time;
        private string description;
        private readonly AlarmService alarmService = new AlarmService();

        public NewItemViewModel()
        {
            SaveCommand = new Command(async () => await OnSave(), ValidateSave);
            CancelCommand = new Command(OnCancel);

            PropertyChanged +=
                (_, e) =>
                {
                    if (e.PropertyName == nameof(Time))
                    {
                        SaveCommand.ChangeCanExecute();
                    }
                };
        }

        public TimeSpan Time
        {
            get => time;
            set
            {
                SetProperty(ref time, value);
                OnPropertyChanged(nameof(Time)); // Уведомление об изменении свойства
            }
        }

        private bool ValidateSave()
        {
            return true;
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
            // Это закроет текущую страницу в стеке навигации
            await Shell.Current.GoToAsync("..");
        }

        private async Task OnSave()
        {
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Time = Time,
                Description = Description
            };

            await DataStore.AddItemAsync(newItem);

            // Это закроет текущую страницу в стеке навигации
            await Shell.Current.GoToAsync("..");

            // Включим таймер для будильника
            alarmService.SetAlarmTimer(newItem);
        }
    }
}
