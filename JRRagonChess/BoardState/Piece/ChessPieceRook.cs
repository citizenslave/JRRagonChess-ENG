using System.Collections.Generic;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.Constants;
using JRRagonGames.JRRagonChess.Types;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceRook : ChessPieceBase {
        public ChessPieceRook(int team, Position position)
            : base(ChessPieceRookId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> moves = new List<ChessMove>();
            
            foreach (int moveOffset in moveOffsets[4..]) {
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

            if (new List<int>(moveOffsets[4..]).FindIndex(offset => (move.EndPosition.Index - move.StartPosition.Index) % offset == 0) == -1) return false;
            //moveOffsets[4..].Any(offset => (move.EndPosition.Index - move.StartPosition.Index) % offset == 0)
            return true;
        }
    }
}
