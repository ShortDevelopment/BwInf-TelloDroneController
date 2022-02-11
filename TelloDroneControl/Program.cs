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
            Controller.SendCmd("takeoff");

            while (true)
            {
                var key = Console.ReadKey().Key;
                int horizontal = 0;
                int horizontal2 = 0;
                if (key == ConsoleKey.A)
                    horizontal = -movement;
                if (key == ConsoleKey.D)
                    horizontal = movement;

                if (key == ConsoleKey.W)
                    horizontal2 = movement;
                if (key == ConsoleKey.S)
                    horizontal2 = -movement;
                Controller.SendCmd($"rc {horizontal} {horizontal2} 0 0");
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
