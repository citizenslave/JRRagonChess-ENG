using static JRRagonGames.Utilities.BitUtilities;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int HalfTurnOffset = 0x08;
        private const int HalfTurnMask = 0x00007f00; // 7 << 8

        public int HalfTurn {
            get => GetBits(
                GameData,
                HalfTurnOffset,
                HalfTurnMask
            );
            set => GameData = SetBits(
                GameData,
                PadBits(value, HalfTurnOffset),
                HalfTurnMask
            );
        }
    }
}