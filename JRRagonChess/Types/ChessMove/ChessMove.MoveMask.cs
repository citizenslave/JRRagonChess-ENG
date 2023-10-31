namespace JRRagonGames.JRRagonChess.Types {
    public readonly partial struct ChessMove {
        private static class MoveMask {
            public const int StartIndexMask = 0b111111 << MoveOffset.StartIndexOffset;
            public const int EndIndexMask = 0b111111 << MoveOffset.EndIndexOffset;
            public const int FlagMask = 0b1111 << MoveOffset.FlagIndexOffset;
        }
    }
}
