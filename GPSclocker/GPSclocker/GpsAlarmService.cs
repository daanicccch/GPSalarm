using GPSclocker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;


namespace GPSclocker
{
    class GpsAlarmService
    {
        public async Task<Location> GetLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    return location;
                }
                else
                {
                    Console.WriteLine("No location data available");
                    return null;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Console.WriteLine($"Feature not supported: {fnsEx.Message}");
                return null;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Console.WriteLine($"Permission error: {pEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
