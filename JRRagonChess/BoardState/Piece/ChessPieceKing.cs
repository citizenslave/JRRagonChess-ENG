﻿using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKing : ChessPieceBase {
        public ChessPieceKing(int team, Position position, Board board)
            : base(ChessPieceKingId, team, position, board) { }

        private static readonly int[] queensideCastleOffsets = new int[] { -1, -2, -3 },
            kingsideCastleOffsets = new int[] { 1, 2 };



        protected override List<ChessMove> GetPseudoLegalMovesForPiece(bool quiet = true) {
            List<ChessMove> legalMoves = GetFixedOffsetMoves(moveOffsets);
            if (quiet && !IsInCheck()) legalMoves.AddRange(GetCastleMoves());

            return legalMoves;
        }

        private bool IsInCheck() => IsCastleTraversalPositionThreatened(piecePosition);

        private List<ChessMove> GetCastleMoves() {
            List<ChessMove> castleMoves = new List<ChessMove>();

            if (board.TeamHasCastleRights(chessTeam)) {
                castleMoves.AddRange(GetSideCastleMove(chessTeam, true));
                castleMoves.AddRange(GetSideCastleMove(chessTeam, false));
            }

            return castleMoves;
        }

        private List<ChessMove> GetSideCastleMove(ChessTeam team, bool isQueenside) {
            if (!board.GetCastleRights(team, isQueenside)) return new List<ChessMove>();
            foreach (int offset in isQueenside ? queensideCastleOffsets : kingsideCastleOffsets)
                if (board[piecePosition.OffsetByIndex(offset).Index] != ChessPieceNone) return new List<ChessMove>();



            Position traversalPosition = piecePosition.OffsetByIndex(isQueenside ? -1 : 1);
            if (IsCastleTraversalPositionThreatened(traversalPosition)) return new List<ChessMove>();



            return new List<ChessMove>() {
                new ChessMove(
                    piecePosition,
                    piecePosition.OffsetByIndex(2 * (isQueenside ? -1 : 1)),
                    ChessMove.MoveFlag.Castle
                )
            };
        }

        private bool IsCastleTraversalPositionThreatened(Position traversalPosition) {
            for (int tileIndex = 0; tileIndex < BoardConstants.TileCount; tileIndex++) {
                int nibble = board[tileIndex];
                if (nibble == ChessPieceNone) continue;
                if (GetTeamFromNibble(nibble) == board.ActiveChessTeam) continue;
                if (GetPseudoLegalMovesFromPosition(Position.GetPositionFromIndex(tileIndex), board, false)
                    .FindIndex(m => m.EndPosition == traversalPosition) != -1) return true;
            }
            return false;
        }



        protected override bool IsMoveValid(ChessMove move) {
            if (!base.IsMoveValid(move)) return false;

            return true;
        }
    }
}
