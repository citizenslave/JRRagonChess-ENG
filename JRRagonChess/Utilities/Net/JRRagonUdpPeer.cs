using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JRRagonGames.Utilities.Net {
    public class JRRagonUdpPeer : JRRagonUdpClient {
        private UdpClient udpPeer = new UdpClient();

        private IPEndPoint? peerEndpoint;
        private string peerSessionKey = string.Empty;
        private long lastPeerUpdate;

        public bool IsPeer { get; private set; } = false;
        public event Action<byte[]>? OnConnectionEstablished;

        public bool Connect(string url, ushort udpPort) {
            byte[] keyData = new byte[256];
            new Random().NextBytes(keyData);
            sessionKey = Convert.ToBase64String(keyData);

            if (IsPortOpen(udpPort)) {
                udpPeer.Close();
                udpPeer = new UdpClient(udpPort);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    udpPeer.Client.IOControl(-1744830452, new byte[] { 0, 0, 0, 0 }, null);
                udpPeer.ReceiveAsync().ContinueWith(t => HandleMessage(IsPeer ? t.Result : default));
                IsPeer = true;
            }

            OnPing -= HandleClientPing;
            OnPing += HandleClientPing;

            OnPongReceived -= RefreshUpdate;
            OnPongReceived -= DisconnectPeer;
            OnPongReceived += DisconnectPeer;

            Uri uri = new Uri("udp://" + url);
            Connect(uri.Host, (ushort)uri.Port, sessionKey);
            Console.WriteLine("Connecting...");

            return IsListening;
        }

        private bool IsPortOpen(ushort port) {
            IPGlobalProperties ipProps = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] udpListenerInformation = ipProps.GetActiveUdpListeners();

            foreach (IPEndPoint udpListener in udpListenerInformation)
                if (udpListener.Port == port) return false;

            return true;
        }

        public void Send(string cmd, string msg) {
            //if (cmd != "ping" && cmd != "pong") Console.WriteLine($"{cmd}:*:{msg}");
            byte[] payload = Encoding.UTF8.GetBytes($"{cmd}:{sessionKey}:{msg}");
            if (peerEndpoint == null) {
                if (!IsListening) {
                    return;
                } else {
                    Send(payload);
                }
            } else {
                udpPeer.Send(payload, payload.Length, peerEndpoint);
            }
        }

        public override void Disconnect() {
            Send("disconnected", "");
            DisconnectPeer(new byte[0]);
            base.Disconnect();
        }

        private void DisconnectPeer(byte[] obj) {
            OnPongReceived -= DisconnectPeer;
            OnPongReceived -= RefreshUpdate;
            OnPongReceived += RefreshUpdate;

            peerEndpoint = null;
            peerSessionKey = string.Empty;

            if (obj.Length > 0) {
                peerSessionKey = Encoding.UTF8.GetString(obj).Split(':')[1];
                lastPeerUpdate = DateTime.UtcNow.Ticks;

                OnConnectionEstablished?.Invoke(obj);
                SendPingLoop(true);
            }

            IsPeer = false;
            udpPeer.Close();
            udpPeer = new UdpClient();
        }

        private void HandleMessage(UdpReceiveResult udpReceiveResult) {
            if (!IsPeer || udpReceiveResult == default) return;
            string msg = Encoding.UTF8.GetString(udpReceiveResult.Buffer);
            string[] cmd = msg.Split(':');
            switch (cmd[0]) {
                case "ping":
                    if (sessionKey == cmd[1]) break;
                    else {
                        if (peerSessionKey == string.Empty) {
                            peerEndpoint = udpReceiveResult.RemoteEndPoint;

                            peerSessionKey = cmd[1];
                            lastPeerUpdate = DateTime.UtcNow.Ticks;

                            OnPongReceived -= DisconnectPeer;
                            OnPongReceived -= RefreshUpdate;
                            OnPongReceived += RefreshUpdate;
                            OnConnectionEstablished?.Invoke(udpReceiveResult.Buffer);
                            SendPingLoop(true);
                        } else if (peerSessionKey != cmd[1]) break;

                        Send("pong", lastPeerUpdate.ToString());

                        if (peerSessionKey != string.Empty) break;
                    }

                    break;
                default: MessageReceived(udpReceiveResult.Buffer); break;
            }
            if (IsPeer) udpPeer.ReceiveAsync().ContinueWith(t => HandleMessage(IsPeer ? t.Result : default));
        }

        private void HandleClientPing(byte[] byteData) {
            string[] msgParts = Encoding.UTF8.GetString(byteData).Split(':');
            if (msgParts[1] == peerSessionKey) Send("pong", DateTime.UtcNow.Ticks.ToString());
        }

        private void SendPingLoop(bool continueLoop) {
            if (continueLoop) Task.Delay(500).ContinueWith(t => SendPingLoop(IsPeer || IsListening));

            double lag = new TimeSpan(DateTime.UtcNow.Ticks - lastPeerUpdate).TotalSeconds;
            if (lag < 10 || !continueLoop) return;

            if (lag > 15) ForceDisconnect();
            else Send("ping", lastPeerUpdate.ToString());
        }

        private void RefreshUpdate(byte[] obj) {
            string msg = Encoding.UTF8.GetString(obj);
            //Console.WriteLine($"[{peerEndpoint}]=>pong:*:{msg.Split(':')[2]}");
            lastPeerUpdate = DateTime.UtcNow.Ticks;
        }
    }
}
