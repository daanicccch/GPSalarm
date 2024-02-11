using GPSclocker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GPSclocker.Services
{
    class GpsAlarmService
    {
        private Dictionary<GpsItem, bool> alarmStates = new Dictionary<GpsItem, bool>();

        public void SetAlarmByLocation(GpsItem alarm)
        {
            Location alarmLocation = new Location(alarm.Latitude, alarm.Longitude);
            double distanceThreshold = 100; // Задайте пороговое расстояние в метрах, когда считать, что прибыли в нужное место

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Location currentLocation = GetDeviceLocation();

                if (currentLocation != null && currentLocation.CalculateDistance(alarmLocation, DistanceUnits.Kilometers) < distanceThreshold)
                {
                    if (alarmStates.TryGetValue(alarm, out bool isEnabled) && isEnabled)
                    {
                        ShowMessageBox("Alarm", "You have arrived!");
                        alarm.IsEnabled = false;
                        alarmStates[alarm] = false;
                    }

                    return false; // Остановить таймер после выполнения условия
                }

                return true; // Продолжить таймер до следующей проверки
            });
        }

        public void CancelAlarmByLocation(GpsItem alarm)
        {
            alarmStates[alarm] = false;
        }

        private Location GetDeviceLocation()
        {
            try
            {
                var location = Geolocation.GetLastKnownLocationAsync();
                return location.Result;
            }
            catch (Exception ex)
            {
                // Обработайте ошибку при получении местоположения
                return null;
            }
        }

        private void ShowMessageBox(string title, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await App.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}
