using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;

namespace JxK.HomeAutomation.Controllers
{
    internal class GeoController
    {
        private readonly ResourceLoader _resourceLoader;
        private readonly Geolocator _geolocator;

        private bool _autoDetect;
        private double _latitude;
        private double _longitude;

        public GeoController()
        {
            _resourceLoader = ResourceLoader.GetForViewIndependentUse("GeoResources");

            _geolocator = new Geolocator();

            bool.TryParse(_resourceLoader.GetString("AutoDetect"), out _autoDetect);            
            double.TryParse(_resourceLoader.GetString("Latitude"), out _latitude);
            double.TryParse(_resourceLoader.GetString("Longitude"), out _longitude);
        }

        public async Task<GeoCoordinate> GetGeoCoordinateAsync()
        {
            if (_autoDetect)
            {
                var geoposition = await _geolocator.GetGeopositionAsync();

                return new GeoCoordinate(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
            }
            else
            {
                return new GeoCoordinate(_longitude, _latitude);
            }
        }

        public class GeoCoordinate
        {
            public double Latitude { get; }
            public double Longitude { get; }

            public GeoCoordinate(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }
    }
}
