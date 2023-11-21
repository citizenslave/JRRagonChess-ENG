using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JRRagonGames.Utilities.Net {
    public class JRRagonNetClient : JRRagonUdpClient {
        public struct HttpJsonResponse {
            public HttpStatusCode status;
            public JsonElement json;
        }

        private static readonly HttpClient httpClient = new HttpClient();

        public string CurrentHttpUrl { get; private set; } = string.Empty;
        public string CurrentApiUrl { get; private set; } = string.Empty;



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

                OnPing -= SendPong;
                Connect(
                    new Uri(url).Host,
                    readPongData.GetProperty("udpPort").GetUInt16(),
                    readPongData.GetProperty("sessionKey").GetString() ?? string.Empty
                );
                OnPing += SendPong;
            } catch { }

            return IsListening;
        }

        private void SendPong(byte[] byteData) => PostHttpRequest(
            $"/api/{CurrentApiUrl}/pong",
            new JsonObject {
                ["sessionKey"] = Encoding.UTF8.GetString(byteData).Split(':')[1],
            }
        );

        public override void Disconnect() => PostHttpRequest(
            $"/api/{CurrentApiUrl}/disconnect",
            new JsonObject { ["sessionKey"] = sessionKey }
        ).ContinueWith(t => base.Disconnect());



        private Task<HttpJsonResponse> SendHttpRequest(HttpRequestMessage request) {
            Task<HttpResponseMessage> response = httpClient.SendAsync(request);
            Task<Stream> responseStream = response.ContinueWith(t => t.Result.Content.ReadAsStreamAsync()).Result;
            return responseStream.ContinueWith(t => new HttpJsonResponse {
                status = response.Result.StatusCode,
                json = JsonDocument.Parse(t.Result).RootElement,
            });
        }

        protected Task<HttpJsonResponse> PostHttpRequest(string url, JsonObject requestBody) =>
            PostHttpRequest(url, requestBody.ToJsonString());

        protected Task<HttpJsonResponse> PostHttpRequest(string url, string requestBody) =>
            SendHttpRequest(new HttpRequestMessage(HttpMethod.Post, new Uri($"{CurrentHttpUrl}{url}")) {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
            });
    }
}
