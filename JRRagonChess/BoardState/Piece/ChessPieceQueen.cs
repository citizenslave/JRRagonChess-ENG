using JRRagonGames.JRRagonChess.Types;
using System.Collections.Generic;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceQueen : ChessPieceBase {
        public ChessPieceQueen(int team, Position position)
            : base(ChessPieceQueenId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> moves = new List<ChessMove>();

            foreach (int moveOffset in moveOffsets) {
                for (int searchIndex = PiecePosition.Index; IsValidSquare(searchIndex, moveOffset); searchIndex += moveOffset) {
                    int targetIndex = searchIndex + moveOffset;
                    Position targetPosition = Position.GetPositionFromIndex(targetIndex);
                    int pieceNibbleAtTarget = currentBoardState[searchIndex + moveOffset];

                    if (pieceNibbleAtTarget == ChessPieceNone || GetPieceTeamRaw(pieceNibbleAtTarget) != PieceTeam)
                        moves.Add(new ChessMove(PiecePosition, targetPosition));
                    if (pieceNibbleAtTarget != ChessPieceNone) break;
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
