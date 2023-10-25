using JRRagonGames.JRRagonChess.BoardState;
using JRRagonGames.JRRagonChess.BoardState.Piece;

namespace JRRagonGames.JRRagonChess {
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
}
