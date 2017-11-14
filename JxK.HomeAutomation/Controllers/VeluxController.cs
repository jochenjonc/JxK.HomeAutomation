using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace JxK.HomeAutomation.Controllers
{
    internal class VeluxController
    {
        private GpioController _gpioController;

        private GpioPin _gpioPinUp;
        private GpioPin _gpioPinStop;
        private GpioPin _gpioPinDown;

        public VeluxController(GpioController gpioController)
        {
            _gpioController = gpioController ?? throw new ArgumentNullException("gpioController");

            // Initialize GpioPins
            _gpioPinUp = gpioController.OpenPin(GpioPinUp);
            _gpioPinUp.Write(DefaultRelayOpenValue);
            _gpioPinUp.SetDriveMode(GpioPinDriveMode.Output);

            _gpioPinStop = gpioController.OpenPin(GpioPinStop);
            _gpioPinStop.Write(DefaultRelayOpenValue);
            _gpioPinStop.SetDriveMode(GpioPinDriveMode.Output);                      

            _gpioPinDown = gpioController.OpenPin(GpioPinDown);
            _gpioPinDown.Write(DefaultRelayOpenValue);
            _gpioPinDown.SetDriveMode(GpioPinDriveMode.Output);
                      
        }

        // TODO: setters
        public int GpioPinUp => 26;
        public int GpioPinStop => 20;
        public int GpioPinDown => 21;

        public GpioPinValue DefaultRelayOpenValue => GpioPinValue.High; // Value where the relay is OPEN, thus no power is send to the remote

        public void Up()
        {
            _gpioPinUp.Write(RelayClosed());
            Task.Delay(-1).Wait(500);
            _gpioPinUp.Write(RelayOpen());
        }

        public void Stop()
        {
            _gpioPinStop.Write(RelayClosed());
            Task.Delay(-1).Wait(500);
            _gpioPinStop.Write(RelayOpen());
        }

        public void Down()
        {
            _gpioPinDown.Write(RelayClosed());
            Task.Delay(-1).Wait(500);
            _gpioPinDown.Write(RelayOpen());
        }

        private GpioPinValue RelayOpen()
        {
            return DefaultRelayOpenValue;
        }

        private GpioPinValue RelayClosed()
        {
            return DefaultRelayOpenValue == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High;
        }
    }
}
