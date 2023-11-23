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
        protected event Action<byte[]>? OnPongReceived;
        public event Action<byte[]>? OnMessageReceived;
        public event Action? OnDisconnected;

        public bool Connect(string url, ushort udpPort, string _sessionKey) {
            try {
                udpClient.Close();
                udpClient = new UdpClient();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    udpClient.Client.IOControl(-1744830452, new byte[] { 0, 0, 0, 0 }, null);
                udpClient.Connect(url, udpPort);
                udpClient.ReceiveAsync().ContinueWith(t => HandleMessage(!t.IsFaulted && IsListening ? t.Result : default));

                sessionKey = _sessionKey;
                IsListening = true;

                Send(Encoding.UTF8.GetBytes($"ping:{sessionKey}:connect"));
            } catch (Exception e) { Console.WriteLine(e.Message); }

            return IsListening;
        }

        protected void ConnectionEstablished(byte[] data) => OnPongReceived?.Invoke(data);
        protected void MessageReceived(byte[] data) => ProcessMessage(data);
        protected void Send(byte[] data) => udpClient.Send(data, data.Length);

        private void HandleMessage(UdpReceiveResult result) {
            if (!IsListening || result == default) return;
            ProcessMessage(result.Buffer);

            if (!IsListening) return;

            udpClient.ReceiveAsync().ContinueWith(t => HandleMessage(!t.IsFaulted && IsListening ? t.Result : default));
        }

        private void ProcessMessage(byte[] byteData) {
            string cmd = Encoding.UTF8.GetString(byteData).Split(':')[0];

            if (cmd == "ping") OnPing?.Invoke(byteData);
            else if (cmd == "pong") OnPongReceived?.Invoke(byteData);
            else if (cmd == "disconnected") { Disconnect();  OnDisconnected?.Invoke(); }
            else OnMessageReceived?.Invoke(byteData);
        }

        public virtual void Disconnect() {
            udpClient.Close();
            udpClient = new UdpClient();

            sessionKey = string.Empty;
            IsListening = false;
        }

        protected void ForceDisconnect() => OnDisconnected?.Invoke();
    }
}
