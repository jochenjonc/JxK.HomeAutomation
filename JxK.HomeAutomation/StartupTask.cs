using JxK.HomeAutomation.Controllers;
using System;
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
        private Geoposition _position;
        private VeluxController _veluxController;
        private MailController _mailController;
        private SettingsController _settingsController;
        private ThreadPoolTimer _timer;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            _deferral = taskInstance.GetDeferral();

            // Get notified when this task is being terminated by the OS
            taskInstance.Canceled += TaskInstance_Canceled;

            // Create geolocator object
            var geolocator = new Geolocator();

            // Make the request for the current position
            _position = await geolocator.GetGeopositionAsync();

            // Create a Velux Controller
            _veluxController = new VeluxController(GpioController.GetDefault());

            // Create a Mail Controller
            _mailController = new MailController()
            {
                SubjectPrefix = "[HomeAutomation]"
            };

            // Create a Settings Controller
            _settingsController = new SettingsController();

            // Setup a simple timer for testing/demo purposes
            _timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMinutes(10));

            // IDEA: Use UWP to register a timed event
            //BackgroundTaskRegistration task = RegisterBackgroundTask(entryPoint, taskName, hourlyTrigger, userCondition);
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Handle cancellation
            _deferral.Complete();
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            // Get the current time
            var currentTime = DateTimeOffset.Now;

            // Margin (there is already light before sunrise and there is still light after sunset)
            var margin = TimeSpan.FromMinutes(20);

            // Calculate sunrise & sunset for the current day
            var solarTimes = new SolarTimes(currentTime, _position.Coordinate.Point.Position.Latitude, _position.Coordinate.Point.Position.Longitude);
            var sunrise = new DateTimeOffset(solarTimes.Sunrise);
            var sunset = new DateTimeOffset(solarTimes.Sunset);

            // Has the sun already risen today (minus the margin)?
            // => No, do nothing
            // => Yes and is it after 7h45 on a working day?
            //    => No, do nothing
            //    => Yes and have we already opened the blinds today?
            //       => No, open the blinds
            //       => Yes, do nothing
            if (sunrise.Subtract(margin) < currentTime)
            {
                // TODO: Add holidays
                if (currentTime.DayOfWeek != DayOfWeek.Saturday && currentTime.DayOfWeek != DayOfWeek.Sunday && currentTime.TimeOfDay >= new TimeSpan(7, 45, 0))
                {
                    if (_settingsController.LastOpeningTime == null || _settingsController.LastOpeningTime.Value.Date != currentTime.Date)
                    {
                        _mailController.Send("Opening the Velux blinds", $"Sunrise: {sunrise.ToString("F")}\r\nMargin: {margin.ToString("%m' min.'")}\r\n\r\nCurrent time: {currentTime.ToString("F")}\r\n\r\nLocation: http://www.google.com/maps/place/{_position.Coordinate.Point.Position.Latitude},{_position.Coordinate.Point.Position.Longitude}");
                        _veluxController.Up();
                        _settingsController.LastOpeningTime = currentTime;
                    }
                }
            }


            // TODO: Do not open & close the blinds in the same interval

            
            // Has the sun already set today?
            // => No, do nothing
            // => Yes and have we already closed the blinds today?
            //    => No, close the blinds
            //    => Yes, do nothing
            if (sunset.Add(margin) < currentTime)
            {
                if (_settingsController.LastClosingTime == null || _settingsController.LastClosingTime.Value.Date != currentTime.Date)
                {
                    _mailController.Send("Closing the Velux blinds", $"Sunset: {sunset.ToString("F")}\r\nMargin: {margin.ToString("%m' min.'")}\r\n\r\nCurrent time: {currentTime.ToString("F")}\r\n\r\nLocation: http://www.google.com/maps/place/{_position.Coordinate.Point.Position.Latitude},{_position.Coordinate.Point.Position.Longitude}");
                    _veluxController.Down();
                    _settingsController.LastClosingTime = currentTime;
                }
            }
        }
    }
}