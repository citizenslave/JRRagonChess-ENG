namespace JRRagonGames.JRRagonChess.Types {
    public readonly struct CastleRights {
        public const byte WhiteKings = 0x00;
        public const byte WhiteQueen = 0x01;
        public const byte BlackKings = 0x02;
        public const byte BlackQueen = 0x03;



        private readonly bool[] rights;



        private CastleRights(bool[] _rights) { rights = _rights; }
        public CastleRights(bool wk, bool wq, bool bk, bool bq)
            : this(new bool[4] { wk, wq, bk, bq }) { }



        public bool this[int idx] {
            get => rights[idx];
            set => rights[idx] = value;
        }
    }
}
