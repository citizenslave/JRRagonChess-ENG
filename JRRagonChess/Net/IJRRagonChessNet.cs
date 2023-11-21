using JRRagonGames.JRRagonChess.Types;

namespace JRRagonGames.JRRagonChess.Net {
    public interface IJRRagonChessNet {
        public ChessTeam AssignedTeam { get; }
        public bool IsListening { get; }

        public void PostMove(ChessMove move);
        public void Rematch(string uci, ChessTeam team, bool confirm = false);
        public void ConfirmRematch();
    }
}
