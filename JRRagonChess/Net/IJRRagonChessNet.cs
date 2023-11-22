using JRRagonGames.JRRagonChess.Types;
using System;

namespace JRRagonGames.JRRagonChess.Net {
    public interface IJRRagonChessNet {
        public ChessTeam AssignedTeam { get; }
        public bool IsListening { get; }

        public event Action<string>? OnSetUciPosition;
        public event Action<ChessMove>? OnMakeMove;
        public event Action<string, ChessTeam>? OnRematchRequest;
        public event Action OnDisconnected;

        public void Disconnect();
        public void PostMove(ChessMove move);
        public void Rematch(string uci, ChessTeam team, bool confirm = false);
        public void ConfirmRematch();
    }
}
