using Xamarin.Forms;

namespace GPSclocker
{
    public class AutoOpenTimePicker : TimePicker
    {
        public AutoOpenTimePicker()
        {
            // При создании элемента, сразу открываем TimePicker
            this.Focused += (sender, e) => { this.Focus(); };
        }
    }
}
