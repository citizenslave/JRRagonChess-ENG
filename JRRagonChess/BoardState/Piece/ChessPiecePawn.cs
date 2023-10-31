using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;
using static JRRagonGames.JRRagonChess.Types.ChessMove.MoveFlag;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPiecePawn : ChessPieceBase {
        public ChessPiecePawn(int team, Position position)
            : base(ChessPiecePawnId, team, position) { }

        private static readonly int[] captureOffsets = new[] { 7, 9 },
            pushOffsets = new[] { 8, 16 },
            homeRanks = new[] { 1, 6 },
            promotionRanks = new[] { 7, 0 },
            promotionOptions = new int[] {
                KnightPromotion,
                BishopPromotion,
                RookPromotion,
                QueenPromotion,
            };



        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            int directionMultiplier = Board.TeamDirectionMultiplier((ChessTeam)teamIndex);

            List<ChessMove> legalMoves = GetCaptureMoves(currentBoardState, directionMultiplier);
            legalMoves.AddRange(GetPushMoves(currentBoardState, directionMultiplier));

            List<ChessMove> promotionMoves = new List<ChessMove>();
            foreach (ChessMove move in legalMoves) promotionMoves.AddRange(GetPromotionMoves(move));

            return promotionMoves;
        }



        private List<ChessMove> GetCaptureMoves(Board currentBoardState, int directionMultiplier) =>
            GetFixedOffsetMoves(currentBoardState, captureOffsets, directionMultiplier)
                .FindAll(FilterValidCaptures(currentBoardState))
                .ConvertAll(SetEnPassantFlag(currentBoardState));

        private static Predicate<ChessMove> FilterValidCaptures(Board currentBoardState) =>
            m => currentBoardState[m.EndPosition.Index] != ChessPieceNone ||
                IsTargetingEnPassant(currentBoardState, m.EndPosition);

        private static Converter<ChessMove, ChessMove> SetEnPassantFlag(Board currentBoardState) =>
            m => !IsTargetingEnPassant(currentBoardState, m.EndPosition) ? m
                : new ChessMove(m.StartPosition, m.EndPosition, EnPassant);

        private static bool IsTargetingEnPassant(Board currentBoardState, Position targetPosition) =>
            currentBoardState.HasEnPassant && targetPosition.Index == currentBoardState.EnPassantIndex;



        private List<ChessMove> GetPushMoves(Board currentBoardState, int directionMultiplier) {
            List<ChessMove> pushMoves = new List<ChessMove>();

            foreach (int moveOffset in pushOffsets) {
                Position targetPosition = piecePosition.OffsetByIndex(moveOffset * directionMultiplier);
                if (currentBoardState[targetPosition.Index] != ChessPieceNone) break;

                int flag = moveOffset == pushOffsets[1] ? DoublePush : NoMoveFlag;
                ChessMove move = new ChessMove(piecePosition, targetPosition, flag);

                pushMoves.Add(move);

                if (piecePosition.rank != homeRanks[teamIndex]) break;
            }

            return pushMoves;
        }




        private List<ChessMove> GetPromotionMoves(ChessMove move) {
            int targetIndex = move.EndPosition.Index;
            List<ChessMove> legalMoves = new List<ChessMove>();

            if (Position.GetRankFromIndex(targetIndex) == promotionRanks[teamIndex])
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

            if (!new List<int>(captureOffsets).Contains(adjustedMoveOffset)) return false;

            if (currentBoardState.HasEnPassant && currentBoardState.EnPassantIndex == move.EndPosition.Index) return true;
            return isTargetOccupied;
        }
    }
}
