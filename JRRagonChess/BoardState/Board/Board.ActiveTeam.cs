using JRRagonGames.JRRagonChess.Types;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int ActiveTeamMask = 0x10000000; // 1 << 28



        public ChessTeam ActiveChessTeam {
            get => (ChessTeam)gameDataRegister[ActiveTeamMask];
            set => gameDataRegister[ActiveTeamMask] = (int)value;
        }



        public ChessTeam OtherChessTeam => (ChessTeam)((int)ActiveChessTeam ^ 1);

        public int ActiveTeamIndex => (int)ActiveChessTeam;
        public int OtherTeamIndex => (int)OtherChessTeam;



        public static int TeamDirectionMultiplier(ChessTeam team) => team == ChessTeam.WhiteTeam ? 1 : -1;
    }
}
