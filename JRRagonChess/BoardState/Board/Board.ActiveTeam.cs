using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.Utilities.BitUtilities;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int ActiveTeamMask = 0x00008000; // 1 << 15



        public ChessTeam ActiveChessTeam {
            get => (ChessTeam)gameDataRegister[ActiveTeamMask];
            set => gameDataRegister[ActiveTeamMask] = (int)value;
        }
        public ChessTeam OtherChessTeam {
            get => (ChessTeam)((int)ActiveChessTeam ^ 1);
        }



        public int TeamRankMultiplier => (int)ActiveChessTeam;



        public static int TeamDirectionMultiplier(ChessTeam team) => team == ChessTeam.WhiteTeam ? 1 : -1;
    }
}