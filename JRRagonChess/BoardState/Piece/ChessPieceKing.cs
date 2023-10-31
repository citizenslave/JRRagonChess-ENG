using System.Collections.Generic;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.Constants;
using static JRRagonGames.JRRagonChess.Types.Position;
using System;
using JRRagonGames.JRRagonChess.Types;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKing : ChessPieceBase {
        public ChessPieceKing(int team, Position position)
            : base(ChessPieceKingId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> moves = new List<ChessMove>();

            foreach (int moveOffset in moveOffsets) {
                int targetIndex = PiecePosition.OffsetByIndex(moveOffset).Index;
                if (targetIndex < 0 || targetIndex >= TileCount) continue;

                Position targetPosition = GetPositionFromIndex(targetIndex);
                int toFile = targetPosition.file, fromFile = PiecePosition.file;
                if (Math.Abs(toFile - fromFile) > 1) continue;

                int pieceNibbleAtTarget = currentBoardState[targetIndex];
                if (pieceNibbleAtTarget == 0 || GetPieceTeamRaw(pieceNibbleAtTarget) != PieceTeam)
                    moves.Add(new ChessMove(PiecePosition, targetPosition));
            }



            if (currentBoardState.TeamHasCastleRights((ChessTeam)TeamIndex)) {
                if (currentBoardState.GetCastleRights((ChessTeam)TeamIndex, true)) {
                    int[] queensideCastleOffsets = new int[] { -1, -2, -3 };
                    bool isValid = true;
                    foreach (int queensideCastleOffset in queensideCastleOffsets) {
                        int checkIndex = PiecePosition.OffsetByIndex(queensideCastleOffset).Index;
                        if (currentBoardState[checkIndex] != ChessPieceNone) isValid = false;
                    }
                    if (isValid) moves.Add(new ChessMove(PiecePosition, PiecePosition.OffsetByIndex(-2), ChessMove.MoveFlag.Castle));
                }

                if (currentBoardState.GetCastleRights((ChessTeam)TeamIndex, false)) {
                    int[] kingsideCastleOffsets = new int[] { 1, 2 };
                    bool isValid = true;
                    foreach (int kingsideCastleOffset in kingsideCastleOffsets) {
                        int checkIndex = PiecePosition.OffsetByIndex(kingsideCastleOffset).Index;
                        if (currentBoardState[checkIndex] != ChessPieceNone) isValid = false;
                    }
                    if (isValid) moves.Add(new ChessMove(PiecePosition, PiecePosition.OffsetByIndex(2), ChessMove.MoveFlag.Castle));
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
