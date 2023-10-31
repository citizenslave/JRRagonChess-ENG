using JRRagonGames.JRRagonChess.Types;


namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int CastleMask = 0x00f00000; // 4 << 20



        private const int WhiteKingsCastle = 0x00800000;
        private const int WhiteQueenCastle = 0x00400000;
        private const int BlackKingsCastle = 0x00200000;
        private const int BlackQueenCastle = 0x00100000;



        public bool HasCastleRights => gameDataRegister[CastleMask] > 0;



        public CastleRights AllCastleRights {
            get => new CastleRights(
                CastleRightsWhiteKing,
                CastleRightsWhiteQueen,
                CastleRightsBlackKing,
                CastleRightsBlackQueen
            );

            set => gameDataRegister[0 |
                (value[CastleRights.WhiteKings] ? WhiteKingsCastle : 0) |
                (value[CastleRights.WhiteQueen] ? WhiteQueenCastle : 0) |
                (value[CastleRights.BlackKings] ? BlackKingsCastle : 0) |
                (value[CastleRights.BlackQueen] ? BlackQueenCastle : 0) |
            0] = 0xf;
        }



        public bool CastleRightsWhiteKing {
            get => gameDataRegister[WhiteKingsCastle] != 0;
            set => gameDataRegister[WhiteKingsCastle] = value ? 1 : 0;
        }

        public bool CastleRightsWhiteQueen {
            get => gameDataRegister[WhiteQueenCastle] != 0;
            set => gameDataRegister[WhiteQueenCastle] = value ? 1 : 0;
        }

        public bool CastleRightsBlackKing {
            get => gameDataRegister[BlackKingsCastle] != 0;
            set => gameDataRegister[BlackKingsCastle] = value ? 1 : 0;
        }

        public bool CastleRightsBlackQueen {
            get => gameDataRegister[BlackQueenCastle] != 0;
            set => gameDataRegister[BlackQueenCastle] = value ? 1 : 0;
        }



        public void RevokeCastleRights(ChessTeam team, bool isQueenside) =>
            gameDataRegister[team switch {
                ChessTeam.WhiteTeam => isQueenside ? WhiteQueenCastle : WhiteKingsCastle,
                ChessTeam.BlackTeam => isQueenside ? BlackQueenCastle : BlackKingsCastle,
                _ => -1
            }] = 0;

        public void RevokeCastleRights(ChessTeam team) =>
            gameDataRegister[team switch {
                ChessTeam.WhiteTeam => WhiteKingsCastle | WhiteQueenCastle,
                ChessTeam.BlackTeam => BlackKingsCastle | BlackQueenCastle,
                _ => -1
            }] = 0;



        public bool TeamHasCastleRights(ChessTeam team) =>
            gameDataRegister[team switch {
                ChessTeam.WhiteTeam => WhiteKingsCastle | WhiteQueenCastle,
                ChessTeam.BlackTeam => BlackKingsCastle | BlackQueenCastle,
                _ => -1
            }] > 0;
        public bool GetCastleRights(ChessTeam team, bool isQueenside) =>
            gameDataRegister[team switch {
                ChessTeam.WhiteTeam => isQueenside ? WhiteQueenCastle : WhiteKingsCastle,
                ChessTeam.BlackTeam => isQueenside ? BlackQueenCastle : BlackKingsCastle,
                _ => -1
            }] > 0;
    }
}