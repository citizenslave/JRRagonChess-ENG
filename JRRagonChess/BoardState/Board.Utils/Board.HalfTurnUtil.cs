using static JRRagonGames.JRRagonChess.Utilities.BitUtilities;
using static JRRagonGames.JRRagonChess.BoardState.Board.HalfTurnUtil.Constants;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        public static class HalfTurnUtil {
            public static class Constants {
                public const int HalfTurnOffset = 0x08;
                public const int HalfTurnMask = 0x00007f00; // 7 << 8
            }

            public static int PadHalfTurn(int halfTurn) =>
                PadBits(
                    halfTurn,
                    HalfTurnOffset
                );

            public static string UnloadHalfTurns(int gameData) =>
                GetBits(
                    gameData,
                    HalfTurnOffset,
                    HalfTurnMask
                ).ToString();
        }

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