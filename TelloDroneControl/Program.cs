using System;
using System.Linq;
using System.Threading.Tasks;

namespace TelloDroneControl
{
    class Program
    {
        static TelloController Controller { get; } = new();
        static void Main(string[] args)
            => MainAsync(args).GetAwaiter().GetResult();

        const int movement = 25;

        static async Task MainAsync(string[] args)
        {
            Controller.Initialize(false);

            Controller.ResponseReceived += Controller_ResponseReceived;
            Controller.StatsReceived += Controller_StatsReceived;

            Controller.SendCmd("battery?");


            while (true)
            {
                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.T)
                    Controller.SendCmd("takeoff");
                else if (key == ConsoleKey.L)
                    Controller.SendCmd("land");
                else if (key == ConsoleKey.Escape)
                    Controller.SendCmd("emergency");
                else
                {
                    int horizontal = 0;
                    if (key == ConsoleKey.A)
                        horizontal = -movement;
                    if (key == ConsoleKey.D)
                        horizontal = movement;

                    int horizontal2 = 0;
                    if (key == ConsoleKey.W)
                        horizontal2 = movement;
                    if (key == ConsoleKey.S)
                        horizontal2 = -movement;

                    int rotate = 0;
                    if (key == ConsoleKey.LeftArrow)
                        rotate = movement;
                    if (key == ConsoleKey.RightArrow)
                        rotate = -movement;

                    Controller.SendCmd($"rc {horizontal} {horizontal2} 0 {rotate}");
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
