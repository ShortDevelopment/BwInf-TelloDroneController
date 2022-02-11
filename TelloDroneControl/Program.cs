using System;
using System.Threading.Tasks;

namespace TelloDroneControl
{
    class Program
    {
        static TelloController Controller { get; } = new();
        static void Main(string[] args)
            => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            Controller.Initialize();

            Controller.ResponseReceived += Controller_ResponseReceived;
            Controller.StatsReceived += Controller_StatsReceived;

            Controller.SendCmd("takeoff");
            await Task.Delay(2_000);

            Controller.SendExpansionCmd("led 255 0 255");
            Controller.SendExpansionCmd("mled sc");
            await Task.Delay(2_000);
            Controller.SendExpansionCmd("mled g rrrbb0ppp");            
            Controller.SendCmd("cw 90");
            await Task.Delay(2_000);
            Controller.SendCmd("land");

            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "exit")
                    return;

                Controller.SendCmd(cmd);
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
