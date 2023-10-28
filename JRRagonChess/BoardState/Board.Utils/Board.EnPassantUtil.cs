using static JRRagonGames.JRRagonChess.Utilities.BitUtilities;
using static JRRagonGames.JRRagonChess.BoardState.Board.EnPassantUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.EnPassantUtil.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.ActiveTeamUtil.Constants;
using JRRagonGames.JRRagonChess.Types;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        public static class EnPassantUtil {
            public static class Constants {
                public const int EnPassantOffset = 0x10;
                public const int EnPassantMask = 0x000f0000; // 4 << 16
                public const int NoEnPassant = 0x00000000;
                public const int EnPassantActive = 0x00080000;
            }

            public static string LookupEnPassant(int gameData) =>
                "abcdefgh"[GetBits(
                    gameData,
                    EnPassantOffset,
                    EnPassantMask & ~EnPassantActive)] +
                $"{((gameData & BlackTurn) != 0 ? '3' : '6')}";

            public static int GetEnPassant(string file) => file[0] switch {
                'a' => 0x00080000,
                'b' => 0x00090000,
                'c' => 0x000a0000,
                'd' => 0x000b0000,
                'e' => 0x000c0000,
                'f' => 0x000d0000,
                'g' => 0x000e0000,
                'h' => 0x000f0000,
                _ => NoEnPassant,
            };
        }

        public int EnPassant {
            get => GameData & EnPassantMask;
            set => GameData = SetBits(GameData, value, EnPassantMask);
        }

        public bool HasEnPassant {
            get => (GameData & EnPassantActive) > 0;
        }

        public int EnPassantIndex {
            get => Position.GetPositionFromName(
                LookupEnPassant(GameData)
            ).Index;
            set => GameData = SetBits(
                GameData,
                PadBits(Position.GetFileFromIndex(value), EnPassantOffset) | EnPassantActive,
                EnPassantMask
            );
        }

        public string EnPassantName {
            get => LookupEnPassant(GameData);
            set => GameData = SetBits(
                GameData,
                GetEnPassant(value),
                EnPassantMask
            );
        }
    }
}