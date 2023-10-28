using JRRagonGames.JRRagonChess.BoardState;
using JRRagonGames.JRRagonChess.BoardState.Piece;

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
        WhiteTeam = ChessGameTeam.WhiteGameTeam,
        BlackTeam = ChessGameTeam.BlackGameTeam,
        NoneTeam = ChessGameTeam.NoneGameTeam,
    }

    public static class ChessGameTeam {
        public const byte WhiteGameTeam = 0x00;
        public const byte BlackGameTeam = 0x01;
        public const byte NoneGameTeam = 0xFF;
    }

    public static class ChessPieceTeam {
        public const int ChessPieceWhite = ChessPieceBase.Constants.ChessPieceWhite;
        public const int ChessPieceBlack = ChessPieceBase.Constants.ChessPieceBlack;
    }

    public static class ActiveTeam {
        public const int ActiveTeamWhite = Board.ActiveTeamUtil.Constants.WhiteTurn;
        public const int ActiveTeamBlack = Board.ActiveTeamUtil.Constants.BlackTurn;
    }

    public static class BoardConstants {
        public const int FileCount = Board.Constants.FileCount;
        public const int TileCount = Board.Constants.TileCount;
    }

    public static class PieceUtil {
        public const int ChessPieceNone = ChessPieceBase.Constants.ChessPieceNone;

        public const int ChessPieceWhite = ChessPieceBase.Constants.ChessPieceWhite;
        public const int ChessPieceBlack = ChessPieceBase.Constants.ChessPieceBlack;

        public const int TeamIndexOffset = ChessPieceBase.Constants.TeamIndexOffset;

        public static ChessTeam GetTeamFromNibble(int nibble) => ChessPieceBase.GetTeamFromNibble(nibble);
    }
}
