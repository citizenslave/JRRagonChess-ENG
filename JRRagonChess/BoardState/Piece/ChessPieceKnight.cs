﻿using System.Collections.Generic;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.Constants;
using System;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKnight : ChessPieceBase {
        public ChessPieceKnight(int team, Position position)
            : base(ChessPieceKnightId, team, position) { }

        private new readonly int[] moveOffsets = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> moves = new List<ChessMove>();

            foreach (int moveOffset in moveOffsets) {
                int targetIndex = PiecePosition.OffsetByIndex(moveOffset).Index;
                if (targetIndex < 0 || targetIndex >= TileCount) continue;

                Position targetPosition = Position.GetPositionFromIndex(targetIndex);
                int toFile = targetPosition.file, fromFile = PiecePosition.file;
                if (Math.Abs(toFile - fromFile) > 2) continue;

                int pieceNibbleAtTarget = currentBoardState[targetIndex];
                if (pieceNibbleAtTarget == 0 || GetPieceTeam(pieceNibbleAtTarget) != PieceTeam)
                    moves.Add(new ChessMove(PiecePosition, targetPosition));
            }

            return moves;
        }

        protected override bool IsMoveLegal(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveLegal(move, currentBoardState)) return false;

            if (!new List<int>(moveOffsets).Contains(move.EndPosition.Index - move.StartPosition.Index)) return false;

            return true;
        }
    }
}
