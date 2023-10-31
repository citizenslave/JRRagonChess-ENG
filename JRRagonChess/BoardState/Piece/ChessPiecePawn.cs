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
            int directionMultiplier = Board.TeamDirectionMultiplier((ChessTeam)teamIndex);

            List<ChessMove> legalMoves = GetFixedOffsetMoves(currentBoardState, moveOffsets, directionMultiplier)
                .FindAll(m =>
                    currentBoardState[m.EndPosition.Index] != ChessPieceNone ||
                    IsTargetingEnPassant(currentBoardState, m.EndPosition)
                ).ConvertAll(m => !IsTargetingEnPassant(currentBoardState, m.EndPosition) ? m
                    : new ChessMove(m.StartPosition, m.EndPosition, ChessMove.MoveFlag.EnPassant)
                );

            foreach (int moveOffset in pushOffsets) {
                int adjustedOffset = moveOffset * directionMultiplier;
                Position targetPosition = piecePosition.OffsetByIndex(adjustedOffset);
                if (currentBoardState[targetPosition.Index] != ChessPieceNone) break;

                int flag = moveOffset == pushOffsets[1] ? ChessMove.MoveFlag.DoublePush
                    : ChessMove.MoveFlag.NoMoveFlag;
                ChessMove move = new ChessMove(piecePosition, targetPosition, flag);

                legalMoves.Add(move);

                if (piecePosition.rank != homeRanks[teamIndex]) break;
            }

            List<ChessMove> promotionMoves = new List<ChessMove>();
            foreach (ChessMove move in legalMoves) promotionMoves.AddRange(GetPromotionMoves(move));

            return promotionMoves;
        }

        private static bool IsTargetingEnPassant(Board currentBoardState, Position targetPosition) {
            return true &&
                                    currentBoardState.HasEnPassant &&
                                    targetPosition.Index == currentBoardState.EnPassantIndex &&
                                true;
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
