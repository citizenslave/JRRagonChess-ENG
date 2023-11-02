namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int TurnCountMask = 0x00000fff; // 12 << 0



        public int TurnCount {
            get => gameDataRegister[TurnCountMask];
            set => gameDataRegister[TurnCountMask] = value;
        }
    }
}
