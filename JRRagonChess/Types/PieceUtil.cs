using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.Types {
    public static class PieceUtil {
        public const int PieceNone = ChessPieceNone;
        public const int PiecePawn = ChessPiecePawnId;
        public const int PieceKnight = ChessPieceKnightId;
        public const int PieceKing = ChessPieceKingId;
        public const int PieceRook = ChessPieceRookId;
        public const int PieceBishop = ChessPieceBishopId;
        public const int PieceQueen = ChessPieceQueenId;

        public const int PieceWhite = ChessPieceWhite;
        public const int PieceBlack = ChessPieceBlack;



        public static ChessTeam ExtractTeamFromNibble(int nibble) => GetTeamFromNibble(nibble);
        public static int ExtractPieceFromNibble(int nibble) => GetPieceType(nibble);
        public static int GeneratePieceNibble(ChessTeam team, int pieceId) => GetPieceNibble(team, pieceId);
        public static int GetNibbleFromFen(char fenChar) => GetPieceNibble(fenChar);
        public static char GetFenCharFromNibble(int nibble) => GetFenCode(nibble);
    }
}
