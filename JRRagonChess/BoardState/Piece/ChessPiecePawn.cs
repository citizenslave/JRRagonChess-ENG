using JRRagonGames.JRRagonChess.Types;
using System.Collections.Generic;
using static JRRagonGames.JRRagonChess.BoardState.Board;
using static JRRagonGames.JRRagonChess.BoardState.Board.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    public class ChessPiecePawn : ChessPieceBase {
        public ChessPiecePawn(int team, Position position)
            : base(ChessPiecePawnId, team, position) { }

        private new readonly int[] moveOffsets = new[] { 8, 16 };
        private readonly int[] captureOffsets = new[] { 7, 9 };
        private readonly int[] homeRanks = new[] { 1, 6 };

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> legalMoves = new List<ChessMove>();

            int directionMultiplier = TeamDirectionMultiplier((ChessTeam)TeamIndex);

            foreach (int moveOffset in captureOffsets) {
                int adjustedOffset = moveOffset * directionMultiplier;
                int targetIndex = PiecePosition.OffsetByIndex(adjustedOffset).Index;
                if (targetIndex < 0 || targetIndex >= TileCount) continue;
                int pieceNibble = currentBoardState[targetIndex];
                bool targetingPiece = pieceNibble != ChessPieceNone;

                bool targetingOpponent = GetPieceTeamRaw(pieceNibble) != PieceTeam;
                bool targetingEnPassant = targetIndex == currentBoardState.EnPassantIndex && currentBoardState.HasEnPassant;

                int toFile = PiecePosition.file + (-directionMultiplier * 8) + adjustedOffset;
                bool isEdge = toFile < 0 || (toFile > (FileCount - 1));
                
                Position targetPosition = Position.GetPositionFromIndex(targetIndex);
                if ((!isEdge && targetingPiece && targetingOpponent) || targetingEnPassant)
                    legalMoves.AddRange(
                        GetPromotionMoves(
                            new ChessMove(
                                PiecePosition,
                                targetPosition,
                                targetingEnPassant ? ChessMove.MoveFlag.EnPassant
                                    : ChessMove.MoveFlag.NoMoveFlag
                            )
                        )
                    );
            }

            foreach (int moveOffset in moveOffsets) {
                int adjustedOffset = moveOffset * directionMultiplier;
                int targetIndex = PiecePosition.OffsetByIndex(adjustedOffset).Index;
                bool targetOccupied = currentBoardState[targetIndex] != ChessPieceNone;
                if (targetOccupied) break;

                Position targetPosition = Position.GetPositionFromIndex(targetIndex);
                legalMoves.AddRange(GetPromotionMoves(new ChessMove(PiecePosition, targetPosition)));

                if (PiecePosition.rank != homeRanks[TeamIndex]) break;
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
            
            if (Position.GetRankFromIndex(targetIndex) == promotionRanks[TeamIndex])
                legalMoves.AddRange(new List<int>(promotionOptions).ConvertAll(promotionOption =>
                    new ChessMove(move.StartPosition, move.EndPosition, promotionOption)
                ));
            else legalMoves.Add(move);
            
            return legalMoves;
        }

        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            int directionMultiplier = TeamDirectionMultiplier((ChessTeam)TeamIndex);

            int moveIndexOffset = move.EndPosition.Index - move.StartPosition.Index;
            if (moveOffsets[0] != (directionMultiplier * moveIndexOffset)) {
                if (PiecePosition.rank == homeRanks[TeamIndex] && moveOffsets[1] == (directionMultiplier * moveIndexOffset)) return true;
                if (!new List<int>(captureOffsets).Contains(directionMultiplier * moveIndexOffset)) return false;
                if (currentBoardState.EnPassantIndex == move.EndPosition.Index && currentBoardState.HasEnPassant) return true;
                if (currentBoardState[move.EndPosition.Index] == ChessPieceNone) return false;
            }

            return true;
        }
    }
}
