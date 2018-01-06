using JxK.HomeAutomation.Controllers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using Innovative.SolarCalculator;

namespace JxK.HomeAutomation
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private VeluxController _veluxController;
        private ThreadPoolTimer _timer;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            _deferral = taskInstance.GetDeferral();

            // Create geolocator object
            Geolocator geolocator = new Geolocator();

            // Make the request for the current position
            var pos = await geolocator.GetGeopositionAsync();

            // Calculate sunrise & sunset
            var solarTimes = new SolarTimes(DateTimeOffset.Now, pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
            var sunrise = solarTimes.Sunrise;
            var sunset = solarTimes.Sunset;

            // Create a Velux Controller
            _veluxController = new VeluxController(GpioController.GetDefault());

            // Setup a simple timer for testing/demo purposes
            _timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromSeconds(10));


            // TODO: Use UWP to register a timed event
            //BackgroundTaskRegistration task = RegisterBackgroundTask(entryPoint, taskName, hourlyTrigger, userCondition);
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            _veluxController.Up();
            Task.Delay(-1).Wait(2000);

            _veluxController.Stop();
            Task.Delay(-1).Wait(2000);

            _veluxController.Down();
            Task.Delay(-1).Wait(2000);
        }
    }
}
