using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.Utilities.BitUtilities;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int CastleOffset = 0x14;
        private const int CastleMask = 0x00f00000; // 4 << 20

        public const int WhiteKingsCastle = 0x00800000;
        public const int WhiteQueenCastle = 0x00400000;
        public const int BlackKingsCastle = 0x00200000;
        public const int BlackQueenCastle = 0x00100000;

        private const int TeamRightsOffsetMultiplier = 0x02;

        public int CastleRights {
            get => GameData & CastleMask;
            set => GameData = SetBits(GameData, value, CastleMask);
        }



        public bool CastleRightsWhiteKing {
            get => (GameData & WhiteKingsCastle) != 0;
            set => GameData = ToggleBits(GameData, WhiteKingsCastle, value);
        }

        public bool CastleRightsWhiteQueen {
            get => (GameData & WhiteQueenCastle) != 0;
            set => GameData = ToggleBits(GameData, WhiteQueenCastle, value);
        }

        public bool CastleRightsBlackKing {
            get => (GameData & BlackKingsCastle) != 0;
            set => GameData = ToggleBits(GameData, BlackKingsCastle, value);
        }

        public bool CastleRightsBlackQueen {
            get => (GameData & BlackQueenCastle) != 0;
            set => GameData = ToggleBits(GameData, BlackQueenCastle, value);
        }



        public void ToggleCastleRights(ChessTeam team, bool isQueenside, bool value) =>
            GameData = ToggleBits(
                GameData,
                team switch {
                    ChessTeam.WhiteTeam => isQueenside ? WhiteQueenCastle : WhiteKingsCastle,
                    ChessTeam.BlackTeam => isQueenside ? BlackQueenCastle : BlackKingsCastle,
                    _ => 0
                },
                value
            );

        public int CastleRightsTeam(ChessTeam team) =>
            GameData & ((WhiteKingsCastle | WhiteQueenCastle) >> ((int)team * TeamRightsOffsetMultiplier));

        public bool GetCastleRights(ChessTeam team, bool isQueenside) =>
            (GameData & ((isQueenside ? WhiteQueenCastle : WhiteKingsCastle) >> ((int)team * TeamRightsOffsetMultiplier))) > 0;
    }
}