using JRRagonGames.JRRagonChess.Net.Types;
using JRRagonGames.JRRagonChess.Types;
using JRRagonGames.Utilities.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace JRRagonGames.JRRagonChess.Net {
    public class JRRagonChessPeer : JRRagonUdpPeer, IJRRagonChessNet {
        public IReadOnlyList<PendingGameRequest> WaitingGameRequests { get => _pendingGameRequests.AsReadOnly(); }
        private List<PendingGameRequest> _pendingGameRequests = new List<PendingGameRequest>();
        public PendingGameRequest MatchingRequest { get; private set; }
        public ChessTeam AssignedTeam { get; private set; } = ChessTeam.NoneTeam;

        public bool Connect(string url, FindGamePayload gameRequest) {
            gamePreferences = gameRequest;
            OnConnectionEstablished -= FindGame;
            OnMessageReceived -= HandleUdpMessage;
            
            Connect(url, 8008);

            OnConnectionEstablished += FindGame;
            OnMessageReceived += HandleUdpMessage;

            return IsListening;
        }
        private FindGamePayload gamePreferences = new FindGamePayload();

        private void FindGame(byte[] obj) => Send("findGame", gamePreferences.ToString());

        private void HandleUdpMessage(byte[] obj) {
            string[] msgParts = Encoding.UTF8.GetString(obj).Split(':');
            switch (msgParts[0]) {
                case "findGame":
                    MatchGame(
                        msgParts[2],
                        int.Parse(msgParts[3]),
                        msgParts[4] != "0",
                        msgParts[5] != "0"
                    );
                    break;
                case "startGame":
                    AssignedTeam = (ChessTeam)int.Parse(msgParts[3]);
                    OnSetUciPosition?.Invoke(msgParts[2]);
                    break;
                case "makeMove": OnMakeMove?.Invoke(new ChessMove(ushort.Parse(msgParts[2]))); break;
                case "rematch":
                    pendingRematch = string.Join(':', msgParts);
                    OnRematchRequest?.Invoke(msgParts[2], (ChessTeam)int.Parse(msgParts[3]));
                    break;
                default:
                    Console.WriteLine("Unhandled UDP Message:\n" +
                        $"{msgParts[0]}:*:{string.Join(':', msgParts[2..])}");
                    break;
            }
        }

        private void MatchGame(string position, int chessTeam, bool requirePosition, bool requireTeam) {
            _pendingGameRequests.Clear();
            _pendingGameRequests.Add(new PendingGameRequest {
                team = requireTeam ? (ChessTeam)chessTeam : ChessTeam.NoneTeam,
                uciPosition = string.IsNullOrEmpty(position) ? requirePosition ? "~position startpos moves " : "open" : "open",
            });

            if ((ChessTeam)chessTeam == ChessTeam.NoneTeam) {
                if (requireTeam || (ChessTeam)gamePreferences.teamIndex == ChessTeam.NoneTeam) return;
                chessTeam = gamePreferences.teamIndex ^ 1;
            }

            if (requirePosition && string.IsNullOrEmpty(position)) position = "position startpos moves ";
            if (gamePreferences.requirePosition && string.IsNullOrEmpty(gamePreferences.position))
                gamePreferences.position = "position startpos moves ";

            if (requireTeam && gamePreferences.requireTeam && chessTeam != (gamePreferences.teamIndex ^ 1)) return;
            if (requirePosition && gamePreferences.requirePosition && position != gamePreferences.position) return;

            if (requireTeam && !gamePreferences.requireTeam) AssignedTeam = (ChessTeam)chessTeam;
            if (!requireTeam && (gamePreferences.requireTeam || AssignedTeam == ChessTeam.NoneTeam))
                AssignedTeam = (ChessTeam)(gamePreferences.teamIndex ^ 1);

            if (!requirePosition && gamePreferences.requirePosition) position = gamePreferences.position;

            Send("startGame", $"{position}:{(int)AssignedTeam ^ 1}");
            MatchingRequest = _pendingGameRequests[0];
            _pendingGameRequests.Clear();
        }

        public event Action<string>? OnSetUciPosition;
        public event Action<ChessMove>? OnMakeMove;
        public event Action<string, ChessTeam>? OnRematchRequest;
        private string pendingRematch = string.Empty;

        public void PostMove(ChessMove move) => Send("makeMove", move.moveData.ToString());
        public void Rematch(string uci, ChessTeam team, bool confirm = false) => Send("rematch", $"{uci}:{(int)team}:{(confirm ? 1 : 0)}");
        public void ConfirmRematch() {
            if (string.IsNullOrEmpty(pendingRematch)) return;

            string[] rematchParts = pendingRematch.Split(':');
            Send("startGame", $"{rematchParts[2]}:{rematchParts[3]}");
            OnSetUciPosition?.Invoke(rematchParts[2]);
            AssignedTeam = (ChessTeam)(int.Parse(rematchParts[3]) ^ 1);
        }
    }
}
