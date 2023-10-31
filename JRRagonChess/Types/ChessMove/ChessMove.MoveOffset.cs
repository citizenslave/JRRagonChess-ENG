namespace JRRagonGames.JRRagonChess.Types {
    public readonly partial struct ChessMove {
        private static class MoveOffset {
            public const int StartIndexOffset = 0x0;
            public const int EndIndexOffset = 0x6;
            public const int FlagIndexOffset = 0xC;
        }
    }
}
