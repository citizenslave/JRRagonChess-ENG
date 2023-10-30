namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int HalfCountMask = 0x00007f00; // 7 << 8

        public int HalfCount {
            get => gameDataRegister[HalfCountMask];
            set => gameDataRegister[HalfCountMask] = value;
        }
    }
}
