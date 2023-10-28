using static JRRagonGames.JRRagonChess.Utilities.BitUtilities;
using static JRRagonGames.JRRagonChess.BoardState.Board.ActiveTeamUtil.Constants;
using JRRagonGames.JRRagonChess.BoardState.Piece;
using JRRagonGames.JRRagonChess.ChessUtils;

namespace JRRagonGames.JRRagonChess.BoardState
{
    public partial class Board { 
        public static class ActiveTeamUtil {
            public static class Constants {
                public const int ActiveTeamOffset = 0x0f;
                public const int ActiveTeamMask = 0x00008000; // 1 << 15
                public const int WhiteTurn = 0x00000000;
                public const int BlackTurn = 0x00008000;
            }

            public static int GetDirectionMultiplier(int team) => team == Constants.WhiteTurn ? 1 : -1;
        }

        public ChessTeam ActiveChessTeam { get => (ChessTeam)(ActiveTeam >> ActiveTeamOffset); }
        public ChessTeam OtherChessTeam {  get => (ChessTeam)(OtherTeam >> ActiveTeamOffset); }

        public int ActiveTeam {
            get => GameData & ActiveTeamMask;
            set => GameData = SetBits(GameData, value, ActiveTeamMask);
        }

        public int ActiveTeamPiece {
            get => ActiveTeam >> ChessPieceBase.Constants.TeamPieceOffset;
        }

        public int OtherTeam {
            get => ActiveTeam ^ ActiveTeamMask;
        }

        public int OtherTeamPiece {
            get => OtherTeam >> ChessPieceBase.Constants.TeamPieceOffset;
        }

        public int TeamRankMultiplier {
            get => ActiveTeam >> ActiveTeamOffset;
        }
    }
}