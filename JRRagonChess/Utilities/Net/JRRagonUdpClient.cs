using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;

namespace JRRagonGames.Utilities.Net {
    public class JRRagonUdpClient {
        private UdpClient udpClient = new UdpClient();

        public bool IsListening { get; private set; } = false;
        protected string sessionKey = string.Empty;



        protected event Action<byte[]>? OnPing;
        public event Action<byte[]>? OnConnectionEstablished;
        public event Action<byte[]>? OnMessageReceived;
        public event Action? OnDisconnected;

        public bool Connect(string url, ushort udpPort, string _sessionKey) {
            try {
                udpClient.Close();
                udpClient = new UdpClient();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    udpClient.Client.IOControl(-1744830452, new byte[] { 0, 0, 0, 0 }, null);
                udpClient.Connect(url, udpPort);
                udpClient.ReceiveAsync().ContinueWith(t => HandleMessage(IsListening ? t.Result.Buffer : new byte[0]));

                sessionKey = _sessionKey;
                IsListening = true;
                OnDisconnected += Disconnect;

                Send(Encoding.UTF8.GetBytes($"ping:{sessionKey}:connect"));
            } catch (Exception e) { Console.WriteLine(e.Message); }

            return IsListening;
        }

        protected void ConnectionEstablished(byte[] data) => OnConnectionEstablished?.Invoke(data);
        protected void MessageReceived(byte[] data) => HandleMessage(data);
        protected void Send(byte[] data) => udpClient.Send(data, data.Length);

        private void ReceiveMessage(IAsyncResult ar) {
            if (!IsListening) return;
            IPEndPoint remoteEndpoint = (IPEndPoint)udpClient.Client.LocalEndPoint;
            byte[] byteData = udpClient.EndReceive(ar, ref remoteEndpoint);

            if (IsListening) {
                HandleMessage(byteData);
                udpClient.BeginReceive(new AsyncCallback(ReceiveMessage), null);
            }
        }

        private void HandleMessage(byte[] byteData) {
            if (!IsListening || byteData.Length == 0) return;

            string cmd = Encoding.UTF8.GetString(byteData).Split(':')[0];

            if (cmd == "ping") OnPing?.Invoke(byteData);
            else if (cmd == "pong") OnConnectionEstablished?.Invoke(byteData);
            else if (cmd == "disconnected") OnDisconnected?.Invoke();
            else OnMessageReceived?.Invoke(byteData);

            if (!IsListening) return;

            udpClient.ReceiveAsync().ContinueWith(t => HandleMessage(IsListening ? t.Result.Buffer : new byte[0]));
        }

        public virtual void Disconnect() {
            udpClient.Close();
            udpClient = new UdpClient();

            OnDisconnected -= Disconnect;
            sessionKey = string.Empty;
            IsListening = false;
        }
    }
}
