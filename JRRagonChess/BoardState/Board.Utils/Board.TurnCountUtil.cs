using static JRRagonGames.JRRagonChess.Utilities.BitUtilities;
using static JRRagonGames.JRRagonChess.BoardState.Board.TurnCountUtil.Constants;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        public static class TurnCountUtil {
            public static class Constants {
                public const int TurnCountOffset = 0x00;
                public const int TurnCountMask = 0x000000ff; // 8 << 0
            }

            public static int PadTurnCount(int turnCount) =>
                PadBits(
                    turnCount,
                    Constants.TurnCountOffset
                );
            public static string UnloadTurnCount(int turnCount) =>
                GetBits(
                    turnCount,
                    Constants.TurnCountOffset,
                    Constants.TurnCountMask
                ).ToString();
        }

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