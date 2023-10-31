using JRRagonGames.JRRagonChess.Types;
using JRRagonGames.Utilities;
using static JRRagonGames.Utilities.BitUtilities;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int EnPassantMask = 0x000f0000; // 4 << 16



        private const int EnPassantActive = 0x8;



        public int EnPassant {
            get => gameDataRegister[EnPassantMask];
            set => gameDataRegister[EnPassantMask] = value;
        }



        public bool HasEnPassant => EnPassant > 0;



        public int EnPassantIndex {
            get => Position.GetPositionFromName(EnPassantName).Index;
            set => gameDataRegister[EnPassantMask] = Position.GetFileFromIndex(value) | EnPassantActive;
        }

        public string EnPassantName {
            get => "abcdefgh"[gameDataRegister[EnPassantMask] & ~EnPassantActive] +
                $"{(OtherChessTeam == ChessTeam.WhiteTeam ? '3' : '6')}";
            set => gameDataRegister[EnPassantMask] = value[0] switch {
                'a' => 0x8,
                'b' => 0x9,
                'c' => 0xa,
                'd' => 0xb,
                'e' => 0xc,
                'f' => 0xd,
                'g' => 0xe,
                'h' => 0xf,
                _ => 0,
            };
        }
    }
}
