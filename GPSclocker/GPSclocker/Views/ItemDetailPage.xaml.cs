using GPSclocker.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace GPSclocker.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
           
        }
    }
}