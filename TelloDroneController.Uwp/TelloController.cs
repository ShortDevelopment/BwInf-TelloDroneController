using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TelloDroneControl
{
    public sealed class TelloController
    {
        #region Consts
        const string TelloIp = "192.168.10.1";
        const int TelloPort = 8889;

        const int LocalCommandResponsePort = 8890;
        #endregion

        public bool IsInitialized { get; private set; } = false;
        public UdpClient Connection { get; private set; }

        public void Initialize(bool useEduSdk = false)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Already initialized!");

            Connection = new UdpClient(LocalCommandResponsePort);
            Connection.Connect(TelloIp, TelloPort);
            Connection.BeginReceive(ReceivedResponse, null);

            // Initialize Tello drone
            SendCmd("command");

            if (useEduSdk)
                SendCmd("[TELLO]");
        }

        public void SendCmd(string cmd)
        {
            Debug.Print($"[{DateTime.Now}] Executing: {cmd}");
            byte[] data = Encoding.ASCII.GetBytes(cmd);
            lock(Connection)
                Connection.Send(data, data.Length);
        }

        public void SendExpansionCmd(string cmd)
            => SendCmd($"EXT {cmd}");

        void ReceivedResponse(IAsyncResult result)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, LocalCommandResponsePort);
            byte[] data = Connection.EndReceive(result, ref endpoint);
            string response = Encoding.ASCII.GetString(data);

            if (response.StartsWith("mid:"))
                StatsReceived?.Invoke(TelloStats.Parse(response));
            else
                ResponseReceived?.Invoke(response);

            Connection.BeginReceive(ReceivedResponse, null);
        }

        public event StatsReceivedEventArgs StatsReceived;
        public delegate void StatsReceivedEventArgs(TelloStats stats);

        public event ResponseReceivedEventArgs ResponseReceived;
        public delegate void ResponseReceivedEventArgs(string response);
    }
}
