using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPiecePawn : ChessPieceBase {
        public ChessPiecePawn(int team, Position position)
            : base(ChessPiecePawnId, team, position) { }

        private new readonly int[] moveOffsets = new[] { 7, 9 };
        private readonly int[] pushOffsets = new[] { 8, 16 }, homeRanks = new[] { 1, 6 };

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> legalMoves = new List<ChessMove>();

            int directionMultiplier = Board.TeamDirectionMultiplier((ChessTeam)teamIndex);

            foreach (int moveOffset in moveOffsets) {
                int adjustedOffset = moveOffset * directionMultiplier;
                if (!IsValidSquare(currentBoardState, this, adjustedOffset, 1)) continue;



                Position targetPosition = piecePosition.OffsetByIndex(adjustedOffset);
                int pieceNibbleAtTarget = currentBoardState[targetPosition.Index];
                bool targetingPiece = pieceNibbleAtTarget != ChessPieceNone;

                bool targetingEnPassant = true &&
                    currentBoardState.HasEnPassant &&
                    targetPosition.Index == currentBoardState.EnPassantIndex &&
                true;

                if (!targetingPiece && !targetingEnPassant) continue;

                int flag = targetingEnPassant ? ChessMove.MoveFlag.EnPassant
                    : ChessMove.MoveFlag.NoMoveFlag;
                ChessMove move = new ChessMove(piecePosition, targetPosition, flag);
                List<ChessMove> promotionMoves = GetPromotionMoves(move);

                legalMoves.AddRange(promotionMoves);
            }

            foreach (int moveOffset in pushOffsets) {
                int adjustedOffset = moveOffset * directionMultiplier,
                    targetIndex = piecePosition.OffsetByIndex(adjustedOffset).Index;

                bool targetOccupied = currentBoardState[targetIndex] != ChessPieceNone;
                if (targetOccupied) break;

                Position targetPosition = Position.GetPositionFromIndex(targetIndex);

                int flag = moveOffset == pushOffsets[1] ? ChessMove.MoveFlag.DoublePush
                    : ChessMove.MoveFlag.NoMoveFlag;
                ChessMove move = new ChessMove(piecePosition, targetPosition, flag);
                List<ChessMove> promotionMoves = GetPromotionMoves(move);

                legalMoves.AddRange(promotionMoves);

                if (piecePosition.rank != homeRanks[teamIndex]) break;
            }

            return legalMoves;
        }

        private List<ChessMove> GetPromotionMoves(ChessMove move) {
            int targetIndex = move.EndPosition.Index;
            List<ChessMove> legalMoves = new List<ChessMove>();

            int[] promotionRanks = new[] { 7, 0 },
                promotionOptions = new int[] {
                        ChessMove.MoveFlag.KnightPromotion,
                        ChessMove.MoveFlag.BishopPromotion,
                        ChessMove.MoveFlag.RookPromotion,
                        ChessMove.MoveFlag.QueenPromotion,
                };

            bool isPromoting = Position.GetRankFromIndex(targetIndex) == promotionRanks[teamIndex];
            if (isPromoting)
                legalMoves.AddRange(
                    new List<int>(promotionOptions).ConvertAll(
                        promotionOption => new ChessMove(move.StartPosition, move.EndPosition, promotionOption)
                    )
                );
            else legalMoves.Add(move);
            
            return legalMoves;
        }

        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            int directionMultiplier = Board.TeamDirectionMultiplier(chessTeam),
                moveIndexOffset = move.EndPosition.Index - move.StartPosition.Index,
                adjustedMoveOffset = directionMultiplier * moveIndexOffset;

            bool isTargetOccupied = currentBoardState[move.EndPosition.Index] != ChessPieceNone;

            if (adjustedMoveOffset == pushOffsets[0]) return !isTargetOccupied;
            if (piecePosition.rank == homeRanks[teamIndex] && adjustedMoveOffset == pushOffsets[1]) return !isTargetOccupied;

            if (!new List<int>(moveOffsets).Contains(adjustedMoveOffset)) return false;

            if (currentBoardState.HasEnPassant && currentBoardState.EnPassantIndex == move.EndPosition.Index) return true;
            return isTargetOccupied;
        }
    }
}
