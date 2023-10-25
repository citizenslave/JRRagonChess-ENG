using static JRRagonGames.JRRagonChess.Utilities.BitUtilities;
using static JRRagonGames.JRRagonChess.BoardState.Board.CastleRightsUtil.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;
using JRRagonGames.JRRagonChess.BoardState.Piece;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        public static class CastleRightsUtil {
            public static class Constants {
                public const int CastleOffset = 0x14;
                public const int CastleMask = 0x00f00000; // 4 << 20

                public const int WhiteKingsCastle = 0x00800000;
                public const int WhiteQueenCastle = 0x00400000;
                public const int BlackKingsCastle = 0x00200000;
                public const int BlackQueenCastle = 0x00100000;

                public const int TeamRightsOffset = 0x02;
            }
        }

        public int CastleRights {
            get => GameData & CastleMask;
            set => GameData = SetBits(GameData, value, CastleMask);
        }

        public int CastleRightsWhiteKing {
            get => GameData & WhiteKingsCastle;
        }

        public int CastleRightsWhiteQueen {
            get => GameData & WhiteQueenCastle;
        }

        public int CastleRightsBlackKing {
            get => GameData & BlackKingsCastle;
        }

        public int CastleRightsBlackQueen {
            get => GameData & BlackQueenCastle;
        }

        public int CastleRightsTeam(int team) =>
            GameData & ((WhiteKingsCastle | WhiteQueenCastle) >> (team >> TeamIndexOffset));

        public bool GetCastleRights(int team, bool isQueenside) =>
            (GameData & ((isQueenside ? WhiteQueenCastle : WhiteKingsCastle) >> (team >> TeamRightsOffset))) > 0;
    }
}