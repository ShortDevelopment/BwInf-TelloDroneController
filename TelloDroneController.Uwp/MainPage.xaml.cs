using System.Diagnostics;
using System.Threading.Tasks;
using TelloDroneControl;
using Windows.Gaming.Input;
using Windows.UI.Xaml.Controls;

namespace TelloDroneController.Uwp
{
    public sealed partial class MainPage : Page
    {
        TelloController Controller = new TelloController();
        public MainPage()
        {
            this.InitializeComponent();
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;

            Controller.Initialize(false);
        }

        private async void Gamepad_GamepadAdded(object sender, Gamepad pad)
        {
            await Task.Run(async () =>
            {
                pad.Vibration = new GamepadVibration()
                {
                    LeftMotor = 0.5,
                    RightMotor = 0.8
                };
                await Task.Delay(1000);
                pad.Vibration = new GamepadVibration();
                while (true)
                {
                    var reading = pad.GetCurrentReading();
                    Debug.Print(reading.LeftThumbstickX.ToString());
                    await Task.Delay(100);
                }
            });
        }
    }
}
