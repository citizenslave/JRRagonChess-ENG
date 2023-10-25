namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    public partial class ChessPieceBase {
        public static class Constants {
            public const int ChessPieceNone = 0;
            public const int ChessPiecePawnId = 0b0001;
            public const int ChessPieceKnightId = 0b0010;
            public const int ChessPieceKingId = 0b0011;
            public const int ChessPieceRookId = 0b0101;
            public const int ChessPieceBishopId = 0b0110;
            public const int ChessPieceQueenId = 0b0111;

            public const int ChessPieceWhite = 0b0000;
            public const int ChessPieceBlack = 0b1000;

            public const int ChessPieceTeamMask = 0b1000;
            public const int ChessPieceTypeMask = 0b0111;

            public const int TeamPieceOffset = 0b1100;
            public const int TeamIndexOffset = 0b0011;

            public const string FenIndex = "-PNK-RBQ-pnk-rbq";
        }
    }
}