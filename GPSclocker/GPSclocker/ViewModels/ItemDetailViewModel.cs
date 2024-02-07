using GPSclocker.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GPSclocker.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private TimeSpan time;
        private string description;
        public ItemDetailViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
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


        public string Id { get; set; }
        private bool ValidateSave()
        {
            return true;
        }

        public TimeSpan Time
        {
            get => time;
            set => SetProperty(ref time, value);
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
            DataStore.DeleteItemAsync(ItemId);
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Time = Time,
                Description = Description
            };

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
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
               // string formattedTime = $"{item.Time.Hours} : {item.Time.Minutes}";
                Time = item.Time;
                Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
