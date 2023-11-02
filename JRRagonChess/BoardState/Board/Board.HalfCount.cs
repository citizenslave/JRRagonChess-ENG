namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int HalfCountMask = 0x000ff000; // 8 << 12



        public int HalfCount {
            get => gameDataRegister[HalfCountMask];
            set => gameDataRegister[HalfCountMask] = value;
        }
    }
}
