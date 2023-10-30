using JRRagonGames.JRRagonChess.BoardState;

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
}
