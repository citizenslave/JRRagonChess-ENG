using JRRagonGames.JRRagonChess.Types;
using System.Collections.Generic;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceBishop : ChessPieceBase {
        public ChessPieceBishop(int team, Position position)
            : base(ChessPieceBishopId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> moves = new List<ChessMove>();

            for (int moveOffsetIndex = 0; moveOffsetIndex < 4; moveOffsetIndex++) {
                int moveOffset = moveOffsets[moveOffsetIndex];
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

        protected override bool IsMoveLegal(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveLegal(move, currentBoardState)) return false;

            return true;
        }
    }
}
