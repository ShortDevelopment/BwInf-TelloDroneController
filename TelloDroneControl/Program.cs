using System;
using System.Threading.Tasks;

namespace TelloDroneControl
{
    class Program
    {
        static TelloController Controller { get; } = new();
        static void Main(string[] args)
            => MainAsync(args).GetAwaiter().GetResult();

        const int movement = 10;

        static async Task MainAsync(string[] args)
        {
            Controller.Initialize(false);

            Controller.ResponseReceived += Controller_ResponseReceived;
            Controller.StatsReceived += Controller_StatsReceived;

            Controller.SendCmd("battery?");

            int horizontal = 0;
            int horizontal2 = 0;
            int vertical = 0;
            int rotate = 0;

            while (true)
            {
                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.T)
                    Controller.SendCmd("takeoff");
                else if (key == ConsoleKey.L)
                    Controller.SendCmd("land");
                else if (key == ConsoleKey.Spacebar)
                {
                    horizontal = 0;
                    horizontal2 = 0;
                    vertical = 0;
                    rotate = 0;
                    Controller.SendCmd("stop");
                }
                else if (key == ConsoleKey.Escape)
                    Controller.SendCmd("emergency");
                else
                {
                    if (key == ConsoleKey.A)
                        horizontal -= movement;
                    if (key == ConsoleKey.D)
                        horizontal += movement;

                    if (key == ConsoleKey.W)
                        horizontal2 += movement;
                    if (key == ConsoleKey.S)
                        horizontal2 -= movement;

                    if (key == ConsoleKey.UpArrow)
                        vertical += movement;
                    if (key == ConsoleKey.DownArrow)
                        vertical -= movement;

                    if (key == ConsoleKey.LeftArrow)
                        rotate += movement;
                    if (key == ConsoleKey.RightArrow)
                        rotate -= movement;

                    Controller.SendCmd($"rc {horizontal} {horizontal2} {vertical} {rotate}");
                }

                //string cmd = Console.ReadLine();
                //if (cmd == "exit")
                //    return;

                //Controller.SendCmd(cmd);
            }
        }

        private static void Controller_StatsReceived(TelloStats stats)
        {

        }

        private static void Controller_ResponseReceived(string response)
        {
            Console.WriteLine(response);
        }
    }
}
