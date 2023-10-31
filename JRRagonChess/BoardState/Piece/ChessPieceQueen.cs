﻿using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceQueen : ChessPieceBase {
        public ChessPieceQueen(int team, Position position)
            : base(ChessPieceQueenId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) =>
            GetSlidingMoves(currentBoardState, moveOffsets);



        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) =>
            base.IsMoveValid(move, currentBoardState) &&
            (new List<int>(moveOffsets).FindIndex(SlidingOffsetSelector(move)) != -1);
    }
}
