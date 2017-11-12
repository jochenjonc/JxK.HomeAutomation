using JxK.HomeAutomation.Controllers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;

namespace JxK.HomeAutomation
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private VeluxController _veluxController;
        private ThreadPoolTimer _timer;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            _deferral = taskInstance.GetDeferral();

            // Create a Velux Controller
            _veluxController = new VeluxController(GpioController.GetDefault());

            // Setup a simple timer for testing/demo purposes
            _timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromSeconds(10));


            // TODO: GetLocation
            // TODO: Calculate sunset time
            // TODO: Use UWP to register a timed event
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
