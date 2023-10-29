using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.Utilities.BitUtilities;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        private const int ActiveTeamOffset = 0x0f;
        private const int ActiveTeamMask = 0x00008000; // 1 << 15
        private const int WhiteTurn = 0x00000000;
        private const int BlackTurn = 0x00008000;

        public int ActiveTeam {
            get => GameData & ActiveTeamMask;
            set => GameData = SetBits(GameData, value, ActiveTeamMask);
        }
        public int OtherTeam => ActiveTeam ^ ActiveTeamMask;



        public ChessTeam ActiveChessTeam => (ChessTeam)(ActiveTeam >> ActiveTeamOffset);
        public ChessTeam OtherChessTeam => (ChessTeam)(OtherTeam >> ActiveTeamOffset);



        public int TeamRankMultiplier => ActiveTeam >> ActiveTeamOffset;



        public static int TeamDirectionMultiplier(ChessTeam team) => team == ChessTeam.WhiteTeam ? 1 : -1;
    }
}