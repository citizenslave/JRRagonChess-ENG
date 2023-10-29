using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.Utilities.BitUtilities;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int CastleOffset = 0x14;
        private const int CastleMask = 0x00f00000; // 4 << 20

        private const int WhiteKingsCastle = 0x00800000;
        private const int WhiteQueenCastle = 0x00400000;
        private const int BlackKingsCastle = 0x00200000;
        private const int BlackQueenCastle = 0x00100000;

        private const int TeamRightsOffsetMultiplier = 0x02;

        public bool HasCastleRights => (GameData & CastleMask) > 0;

        public CastleRights AllCastleRights {
            get => new CastleRights(
                CastleRightsWhiteKing,
                CastleRightsWhiteQueen,
                CastleRightsBlackKing,
                CastleRightsBlackQueen
            );

            set => GameData = SetBits(
                GameData,
                0 |
                    (value[CastleRights.WhiteKings] ? WhiteKingsCastle : 0) |
                    (value[CastleRights.WhiteQueen] ? WhiteQueenCastle : 0) |
                    (value[CastleRights.BlackKings] ? BlackKingsCastle : 0) |
                    (value[CastleRights.BlackQueen] ? BlackQueenCastle : 0) |
                0,
                CastleMask
            );
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



        public void RevokeCastleRights(ChessTeam team, bool isQueenside) =>
            GameData = ToggleBits(
                GameData,
                team switch {
                    ChessTeam.WhiteTeam => isQueenside ? WhiteQueenCastle : WhiteKingsCastle,
                    ChessTeam.BlackTeam => isQueenside ? BlackQueenCastle : BlackKingsCastle,
                    _ => 0
                },
                false
            );
        public void RevokeCastleRights(ChessTeam team) =>
            GameData = ToggleBits(
                GameData,
                team switch {
                    ChessTeam.WhiteTeam => WhiteKingsCastle | WhiteQueenCastle,
                    ChessTeam.BlackTeam => BlackKingsCastle | BlackQueenCastle,
                    _ => 0
                },
                false
            );

        public int GetCastleRights(ChessTeam team) =>
            GameData & ((WhiteKingsCastle | WhiteQueenCastle) >> ((int)team * TeamRightsOffsetMultiplier));
        public bool GetCastleRights(ChessTeam team, bool isQueenside) =>
            (GameData & ((isQueenside ? WhiteQueenCastle : WhiteKingsCastle) >> ((int)team * TeamRightsOffsetMultiplier))) > 0;
    }
}