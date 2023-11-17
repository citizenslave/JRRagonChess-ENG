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
            public int status;
            public JsonElement json;
        }

        private static readonly UdpClient udpClient = new UdpClient();
        private static readonly HttpClient httpClient = new HttpClient();

        private static string? currentHttpUrl;
        private static string? currentApiUrl;



        public static string FindGame(byte[] pongMsg) {
            string sessionKey = Encoding.UTF8.GetString(pongMsg).Split(':')[1];
            Task<HttpJsonResponse> findGameResult = PostHttpRequest(
                new Uri($"{currentHttpUrl}/api/JRRagonChess/findGame"),
                new JsonObject {
                    ["requirePosition"] = false,
                    ["requireTeam"] = false,
                    ["sessionKey"] = sessionKey,
                }
            );

            findGameResult.Wait();

            return $"{findGameResult.Result.status}:{findGameResult.Result.json}";
        }

        public static event Action<byte[]>? OnConnectionEstablished;
        public static event Action<byte[]>? OnMessageReceived;

        public static bool Connect(string url, string api) {
            try {
                currentHttpUrl = url; currentApiUrl = api;
                Task<HttpJsonResponse> pingResponseTask = PostHttpRequest(
                    new Uri($"{url}/api/{api}/pong"),
                    new JsonObject { ["userInfo"] = "GUEST" }
                );
                pingResponseTask.Wait();
                JsonElement readPongData = pingResponseTask.Result.json;

                ushort udpPort = readPongData.GetProperty("udpPort").GetUInt16();
                udpClient.Connect(new Uri(url).Host, udpPort);
                udpClient.BeginReceive(new AsyncCallback(ReceiveMessage), udpClient);

                Send(Encoding.UTF8.GetBytes($"ping:{readPongData.GetProperty("sessionKey").GetString()}"));
            } catch { }

            return udpClient.Client.Connected;
        }

        private static void ReceiveMessage(IAsyncResult ar) {
            UdpClient client = (UdpClient)ar.AsyncState;
            IPEndPoint remoteEndpoint = (IPEndPoint)client.Client.LocalEndPoint;

            byte[] byteData = client.EndReceive(ar, ref remoteEndpoint);
            string cmd = Encoding.UTF8.GetString(byteData).Split(':')[0];
            if (cmd == "ping") SendPong(byteData);
            else if (cmd == "pong") OnConnectionEstablished?.Invoke(byteData);
            else OnMessageReceived?.Invoke(byteData);
            client.BeginReceive(new AsyncCallback(ReceiveMessage), ar.AsyncState);
        }

        private static void Send(byte[] data) => udpClient.Send(data, data.Length);

        private static void SendPong(byte[] byteData) {
            PostHttpRequest(
                new Uri($"{currentHttpUrl}/api/{currentApiUrl}/pong"),
                new JsonObject {
                    ["sessionKey"] = Encoding.UTF8.GetString(byteData).Split(':')[1],
                }
            );
        }

        private static Task<HttpJsonResponse> SendHttpRequest(HttpRequestMessage request) {
            Task<HttpResponseMessage> response = httpClient.SendAsync(request);
            Task<Stream> responseStream = response.ContinueWith(t => t.Result.Content.ReadAsStreamAsync()).Result;
            return responseStream.ContinueWith(t => new HttpJsonResponse {
                status = (int)response.Result.StatusCode,
                json = JsonDocument.Parse(t.Result).RootElement,
            });
        }

        private static Task<HttpJsonResponse> PostHttpRequest(Uri uri, JsonObject requestBody) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri) {
                Content = new StringContent(requestBody.ToJsonString(), Encoding.UTF8, "application/json"),
            };

            return SendHttpRequest(request);
        }
    }
}
