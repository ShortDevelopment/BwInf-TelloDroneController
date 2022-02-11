using System;
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

            Controller.ResponseReceived += Controller_ResponseReceived;
        }

        private async void Controller_ResponseReceived(string response)
        {
            await Dispatcher.RunIdleAsync((x) => StatusTextBlock.Text = response);
        }

        const int maxValue = 100;

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
                    await Dispatcher.RunIdleAsync((x) =>
                    {
                        var reading = pad.GetCurrentReading();
                        ValueTextBlock.Text = reading.LeftThumbstickX.ToString();
                        
                        //if (reading.RightTrigger > 0)
                        //    Controller.SendCmd($"up {(int)Math.Round(40 * reading.RightTrigger)}");
                        //else if (reading.LeftTrigger < 0)
                        //    Controller.SendCmd($"down {(int)Math.Round(40 * reading.LeftTrigger)}");

                        //if (reading.RightThumbstickX > 0.1)
                        //    Controller.SendCmd($"right {(int)Math.Round(10 * Math.Abs(reading.RightThumbstickX))}");
                        //else if (reading.RightThumbstickX < -0.1)
                        //    Controller.SendCmd($"left {(int)Math.Round(10 * Math.Abs(reading.RightThumbstickX))}");

                        int sideways = (int)Math.Round(maxValue * reading.LeftThumbstickX);
                        int forewards = (int)Math.Round(maxValue * reading.LeftThumbstickY);
                        int rotate = (int)Math.Round(90 * reading.RightThumbstickX);
                        Controller.SendCmd($"rc {sideways} {forewards} 0 {rotate}");

                        if (reading.Buttons.HasFlag(GamepadButtons.Y))
                            Controller.SendCmd("takeoff");
                        if (reading.Buttons.HasFlag(GamepadButtons.B))
                            Controller.SendCmd("stop");
                        if (reading.Buttons.HasFlag(GamepadButtons.A))
                            Controller.SendCmd("land");
                    });
                    await Task.Delay(100);
                }
            });
        }

        private void TakeoffAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
            => Controller.SendCmd("takeoff");


        private void LandAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
            => Controller.SendCmd("land");

    }
}
