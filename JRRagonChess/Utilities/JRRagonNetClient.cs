using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JRRagonGames.Utilities {
    public class JRRagonNetClient {
        public struct HttpJsonResponse {
            public HttpStatusCode status;
            public JsonElement json;
        }

        private readonly UdpClient udpClient = new UdpClient();
        private static readonly HttpClient httpClient = new HttpClient();
        public bool IsListening { get; private set; } = false;
        protected string sessionKey = string.Empty;

        public string CurrentHttpUrl { get; private set; } = string.Empty;
        public string CurrentApiUrl { get; private set; } = string.Empty;



        public event Action<byte[]>? OnConnectionEstablished;
        public event Action<byte[]>? OnMessageReceived;
        public event Action? OnDisconnected;

        public bool Connect(string url, string api) {
            try {
                CurrentHttpUrl = url; CurrentApiUrl = api;
                Task<HttpJsonResponse> pingResponseTask = PostHttpRequest(
                    $"/api/{api}/pong",
                    new JsonObject { ["userInfo"] = "GUEST" }
                );
                pingResponseTask.Wait();
                JsonElement readPongData = pingResponseTask.Result.json;
                Console.WriteLine(readPongData.ToString());
                ushort udpPort = readPongData.GetProperty("udpPort").GetUInt16();
                udpClient.Connect(new Uri(url).Host, udpPort);
                udpClient.BeginReceive(new AsyncCallback(ReceiveMessage), udpClient);
                sessionKey = readPongData.GetProperty("sessionKey").GetString() ?? string.Empty;
                IsListening = true;
                OnDisconnected += Disconnected;

                Send(Encoding.UTF8.GetBytes($"ping:{sessionKey}"));
            } catch { }

            return IsListening;
        }

        public virtual void Disconnect() => PostHttpRequest(
            $"/api/{CurrentApiUrl}/disconnect",
            new JsonObject { ["sessionKey"] = sessionKey }
        ).ContinueWith(t => Disconnected());

        private void Disconnected() {
            OnDisconnected -= Disconnected;
            sessionKey = string.Empty;
            IsListening = false;
        }

        private void ReceiveMessage(IAsyncResult ar) {
            UdpClient client = (UdpClient)ar.AsyncState;
            IPEndPoint remoteEndpoint = (IPEndPoint)client.Client.LocalEndPoint;

            byte[] byteData = client.EndReceive(ar, ref remoteEndpoint);
            string cmd = Encoding.UTF8.GetString(byteData).Split(':')[0];
            if (cmd == "ping") SendPong(byteData);
            else if (cmd == "pong") OnConnectionEstablished?.Invoke(byteData);
            else if (cmd == "disconnected") OnDisconnected?.Invoke();
            else OnMessageReceived?.Invoke(byteData);

            if (IsListening) client.BeginReceive(new AsyncCallback(ReceiveMessage), ar.AsyncState);
        }

        private void Send(byte[] data) => udpClient.Send(data, data.Length);

        private void SendPong(byte[] byteData) => PostHttpRequest(
            $"/api/{CurrentApiUrl}/pong",
            new JsonObject {
                ["sessionKey"] = Encoding.UTF8.GetString(byteData).Split(':')[1],
            }
        );



        private Task<HttpJsonResponse> SendHttpRequest(HttpRequestMessage request) {
            Task<HttpResponseMessage> response = httpClient.SendAsync(request);
            Task<Stream> responseStream = response.ContinueWith(t => t.Result.Content.ReadAsStreamAsync()).Result;
            return responseStream.ContinueWith(t => new HttpJsonResponse {
                status = response.Result.StatusCode,
                json = JsonDocument.Parse(t.Result).RootElement,
            });
        }

        protected Task<HttpJsonResponse> PostHttpRequest(string url, JsonObject requestBody) {
            return PostHttpRequest(url, requestBody.ToJsonString());
        }

        protected Task<HttpJsonResponse> PostHttpRequest(string url, string requestBody) {
            Uri uri = new Uri($"{CurrentHttpUrl}{url}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri) {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
            };

            return SendHttpRequest(request);
        }
    }
}
