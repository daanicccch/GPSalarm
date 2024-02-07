using GPSclocker.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GPSclocker
{
    public class AlarmService
    {
        private Dictionary<Item, bool> alarmStates = new Dictionary<Item, bool>();

        public void SetAlarmTimer(Item alarm)
        {
            DateTime alarmDateTime = DateTime.Today.Add(alarm.Time);
            TimeSpan timeUntilAlarm = alarmDateTime - DateTime.Now;

            alarmStates[alarm] = true;

            Device.StartTimer(timeUntilAlarm, () =>
            {
                if (alarmStates.TryGetValue(alarm, out bool isEnabled) && isEnabled)
                {
                    ShowMessageBox("Alarm", "Time to wake up!");
                    alarm.IsEnabled = false;
                    alarmStates[alarm] = false;
                }

                return false;
            });
        }

        public void CancelAlarmTimer(Item alarm)
        {
            alarmStates[alarm] = false;
        }

        private void ShowMessageBox(string title, string message)
        {
            // Используем DisplayAlert для отображения диалогового окна
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}
