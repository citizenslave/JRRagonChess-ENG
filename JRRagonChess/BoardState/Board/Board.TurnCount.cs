using static JRRagonGames.Utilities.BitUtilities;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int TurnCountOffset = 0x00;
        private const int TurnCountMask = 0x000000ff; // 8 << 0

        public int TurnCount {
            get => GetBits(
                GameData,
                TurnCountOffset,
                TurnCountMask
            );
            set => GameData = SetBits(
                GameData,
                PadBits(value, TurnCountOffset),
                TurnCountMask
            );
        }
    }
}