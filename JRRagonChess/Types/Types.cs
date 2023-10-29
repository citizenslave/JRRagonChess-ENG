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

    public readonly struct CastleRights {
        public static readonly int WhiteKings = 0x00;
        public const int WhiteQueen = 0x01;
        public const byte BlackKings = 0x02;
        public const byte BlackQueen = 0x03;



        public readonly bool[] rights;
        public CastleRights(bool wk, bool wq, bool bk, bool bq)
            : this(new bool[4] { wk, wq, bk, bq }) { }
        private CastleRights(bool[] _rights) { rights = _rights; }



        public bool this[int idx] {
            get => rights[idx];
            set => rights[idx] = value;
        }
    }

    public static class PieceUtil {
        public const int PieceNone = ChessPieceNone;
        public const int PiecePawn = ChessPiecePawnId;
        public const int PieceRook = ChessPieceRookId;
        public const int PieceKing = ChessPieceKingId;

        public const int PieceWhite = ChessPieceWhite;
        public const int PieceBlack = ChessPieceBlack;

        public const int TeamPieceOffset = Constants.TeamPieceOffset;

        public static ChessTeam ExtractTeamFromNibble(int nibble) => GetTeamFromNibble(nibble);
        public static int ExtractPieceFromNibble(int nibble) => GetPieceType(nibble);
        public static int GeneratePieceNibble(ChessTeam team, int pieceId) => GetPieceNibble(team, pieceId);
        public static int GetNibbleFromFen(char fenChar) => GetPieceNibble(fenChar);
        public static char GetFenCharFromNibble(int nibble) => GetFenCode(nibble);
    }
}
