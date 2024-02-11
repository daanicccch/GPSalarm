using GPSclocker.Models;
using GPSclocker.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GPSclocker.ViewModels
{
    class GpsItemsViewModel : GpsBaseViewMosel
    {
        private GpsItem _selectedItem;

        public ObservableCollection<GpsItem> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<GpsItem> ItemTapped { get; }

        public GpsItemsViewModel()
        {
            Title = "Cloks";
            Items = new ObservableCollection<GpsItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<GpsItem>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (DataStore == null)
                {
                    Debug.WriteLine("DataStore is null.");
                    return;
                }

                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);

                foreach (var item in items)
                {
                    Items.Add(item);
                }

                Debug.WriteLine($"Loaded {Items.Count} items."); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading items: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public GpsItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(GpsNewItemPage));
        }

        async void OnItemSelected(GpsItem item)
        {
            if (item == null)
                return;

          

            await Shell.Current.GoToAsync($"{nameof(GpsItemDetailPage)}?{nameof(GpsItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}
