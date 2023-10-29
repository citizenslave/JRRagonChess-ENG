using JRRagonGames.JRRagonChess.BoardState;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.Types {
    public enum GameState {
        Running = 0x00,
        Stalemate = 0x01,
        Checkmate = 0x02,
        Progress = 0x03,
        Material = 0x04,
        Pending = 0xFF,
    }

    public enum ChessTeam {
        WhiteTeam = 0x00,
        BlackTeam = 0x01,
        NoneTeam = 0xFF,
    }

    public static class BoardConstants {
        public const int FileCount = Board.Constants.FileCount;
        public const int TileCount = Board.Constants.TileCount;
    }

    public static class PieceUtil {
        public const int ChessPieceNone = Constants.ChessPieceNone;
        public const int ChessPiecePawnId = Constants.ChessPiecePawnId;
        public const int ChessPieceRookId = Constants.ChessPieceRookId;
        public const int ChessPieceKingId = Constants.ChessPieceKingId;

        public const int ChessPieceWhite = Constants.ChessPieceWhite;
        public const int ChessPieceBlack = Constants.ChessPieceBlack;

        public const int TeamIndexOffset = Constants.TeamIndexOffset;
        public const int TeamPieceOffset = Constants.TeamPieceOffset;

        public static ChessTeam ExtractTeamFromNibble(int nibble) => GetTeamFromNibble(nibble);
        public static int ExtractPieceFromNibble(int nibble) => GetPieceType(nibble);
        public static int GeneratePieceNibble(ChessTeam team, int pieceId) => GetPieceNibble((int)team << TeamIndexOffset, pieceId);
    }
}
