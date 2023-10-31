using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKing : ChessPieceBase {
        public ChessPieceKing(int team, Position position)
            : base(ChessPieceKingId, team, position) { }

        /// Uses standard offsets
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



            if (currentBoardState.TeamHasCastleRights((ChessTeam)teamIndex)) {
                if (currentBoardState.GetCastleRights((ChessTeam)teamIndex, true)) {
                    int[] queensideCastleOffsets = new int[] { -1, -2, -3 };
                    bool isValid = true;
                    foreach (int queensideCastleOffset in queensideCastleOffsets) {
                        int checkIndex = piecePosition.OffsetByIndex(queensideCastleOffset).Index;
                        if (currentBoardState[checkIndex] != ChessPieceNone) isValid = false;
                    }
                    if (isValid) legalMoves.Add(new ChessMove(piecePosition, piecePosition.OffsetByIndex(-2), ChessMove.MoveFlag.Castle));
                }

                if (currentBoardState.GetCastleRights((ChessTeam)teamIndex, false)) {
                    int[] kingsideCastleOffsets = new int[] { 1, 2 };
                    bool isValid = true;
                    foreach (int kingsideCastleOffset in kingsideCastleOffsets) {
                        int checkIndex = piecePosition.OffsetByIndex(kingsideCastleOffset).Index;
                        if (currentBoardState[checkIndex] != ChessPieceNone) isValid = false;
                    }
                    if (isValid) legalMoves.Add(new ChessMove(piecePosition, piecePosition.OffsetByIndex(2), ChessMove.MoveFlag.Castle));
                }
            }



            return legalMoves;
        }

        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            return true;
        }
    }
}
