using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKnight : ChessPieceBase {
        public ChessPieceKnight(int team, Position position)
            : base(ChessPieceKnightId, team, position) { }

        private new readonly int[] moveOffsets = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };
        /// Matching capture offsets, home ranks irrelevant

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> legalMoves = new List<ChessMove>();



            foreach (int moveOffset in moveOffsets) {

                if (!IsValidSquare(piecePosition.Index, moveOffset)) continue;

                Position targetPosition = piecePosition.OffsetByIndex(moveOffset);
                int pieceNibbleAtTarget = currentBoardState[targetPosition.Index];

                bool targetingPiece = pieceNibbleAtTarget != ChessPieceNone;
                bool targetingOpponent = targetingPiece && GetTeamFromNibble(pieceNibbleAtTarget) != chessTeam;
                if (targetingPiece && !targetingOpponent) continue;

                legalMoves.Add(new ChessMove(piecePosition, targetPosition));
            }

            return legalMoves;
        }

        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            if (!new List<int>(moveOffsets).Contains(move.EndPosition.Index - move.StartPosition.Index)) return false;

            return true;
        }
    }
}
