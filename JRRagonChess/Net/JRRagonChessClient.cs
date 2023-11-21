using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using JRRagonGames.JRRagonChess.Types;
using JRRagonGames.JRRagonChess.Utilities.Net;

namespace JRRagonGames.JRRagonChess.Net {
    public class JRRagonChessClient : JRRagonNetClient {
        public struct PendingGameRequest {
            public ChessTeam team;
            public string uciPosition;
        }

        public struct FindGamePayload {
            public string position;
            public int teamIndex;
            public bool requirePosition;
            public bool requireTeam;
            public string sessionKey;

            public override readonly string ToString() => JsonSerializer.SerializeToElement(
                this,
                new JsonSerializerOptions { IncludeFields = true }
            ).ToString();
        }

        public IReadOnlyList<PendingGameRequest> WaitingGameRequests { get => _pendingGameRequests.AsReadOnly(); }
        private List<PendingGameRequest> _pendingGameRequests = new List<PendingGameRequest>();
        public PendingGameRequest MatchingRequest { get; private set; }
        public ChessTeam AssignedTeam { get; private set; }

        public JRRagonChessClient Connect(string url, FindGamePayload gameRequest) {
            OnConnectionEstablished -= FindGame;
            OnMessageReceived -= HandleUdpMessage;

            Connect(url, "JRRagonChess");

            OnConnectionEstablished += FindGame;
            OnMessageReceived += HandleUdpMessage;

            return this;
        }
        private FindGamePayload gamePreferences = new FindGamePayload();

        public void FindGame(byte[] pongMsg) => PostHttpRequest(
            $"/api/JRRagonChess/findGame",
            new FindGamePayload() {
                position = gamePreferences.position,
                teamIndex = gamePreferences.teamIndex,
                requirePosition = gamePreferences.requirePosition,
                requireTeam = gamePreferences.requireTeam,
                sessionKey = sessionKey,
            }.ToString()
        ).ContinueWith(t => {
            HttpStatusCode status = t.Result.status;
            JsonElement json = t.Result.json;
            if (status == HttpStatusCode.OK) {
                MatchingRequest = json.Deserialize<PendingGameRequest>();
                _pendingGameRequests.Clear();
            } else if (status == HttpStatusCode.Created) {
                MatchingRequest = default;
                _pendingGameRequests = new List<PendingGameRequest>(json.Deserialize<PendingGameRequest[]>());
            }
        });

        public void PostMove(ChessMove move) => PostHttpRequest(
            $"/api/JRRagonChess/postMove",
            new JsonObject { ["move"] = move.moveData, ["sessionKey"] = sessionKey }
        ).ContinueWith(t => pendingRematch = string.Empty);

        public void Rematch(string uci, ChessTeam team, bool confirm = false) => PostHttpRequest(
            $"/api/JRRagonChess/rematch",
            new JsonObject {
                ["sessionKey"] = sessionKey,
                ["position"] = uci,
                ["teamIndex"] = (int)team,
                ["confirm"] = confirm,
            }
        );

        public void ConfirmRematch() {
            if (string.IsNullOrEmpty(pendingRematch)) return;

            string[] rematchParts = pendingRematch.Split(':');
            Rematch(rematchParts[1], (ChessTeam)int.Parse(rematchParts[2]), true);
        }



        private void HandleUdpMessage(byte[] data) {
            string msg = Encoding.UTF8.GetString(data); pendingRematch = string.Empty;
            Console.WriteLine(msg);
            string[] msgParts = msg.Split(":");
            switch (msgParts[0]) {
                case "startGame": AssignedTeam = (ChessTeam)int.Parse(msgParts[2]); OnSetUciPosition?.Invoke(msgParts[1]); break;
                case "makeMove": OnMakeMove?.Invoke(new ChessMove(ushort.Parse(msgParts[1]))); break;
                case "rematch": pendingRematch = msg; OnRematchRequest?.Invoke(msgParts[1], (ChessTeam)int.Parse(msgParts[2])); break;
                default: Console.WriteLine("Unhandled UDP Message:\n" + msg); break;
            }
        }
        public event Action<string>? OnSetUciPosition;
        public event Action<ChessMove>? OnMakeMove;
        public event Action<string, ChessTeam>? OnRematchRequest;
        private string pendingRematch = string.Empty;
    }
}
