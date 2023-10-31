using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceBishop : ChessPieceBase {
        public ChessPieceBishop(int team, Position position)
            : base(ChessPieceBishopId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> moves = new List<ChessMove>();

            foreach (int moveOffset in moveOffsets[0..4]) {
                for (int searchIndex = piecePosition.Index; IsValidSquare(searchIndex, moveOffset); searchIndex += moveOffset) {
                    int targetIndex = searchIndex + moveOffset,
                        pieceNibbleAtTarget = currentBoardState[targetIndex];

                    bool targetingPiece = pieceNibbleAtTarget != ChessPieceNone,
                        targetingOpponent = targetingPiece && GetTeamFromNibble(pieceNibbleAtTarget) != chessTeam;

                    if (!targetingPiece || targetingOpponent)
                        moves.Add(new ChessMove(piecePosition, Position.GetPositionFromIndex(targetIndex)));
                    if (targetingPiece) break;
                }
            }

            return moves;
        }

        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            return true;
        }
    }
}
