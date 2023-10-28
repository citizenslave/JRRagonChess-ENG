namespace JRRagonGames.JRRagonChess.Types {
    public readonly partial struct ChessMove {
        public static class MoveFlag {
            public const int NoMoveFlag = 0x0;
            public const int EnPassant = 0x1;
            public const int KnightPromotion = 0x2;
            public const int Castle = 0x3;
            public const int DoublePush = 0x4;
            public const int RookPromotion = 0x5;
            public const int BishopPromotion = 0x6;
            public const int QueenPromotion = 0x7;
        }
    }
}
