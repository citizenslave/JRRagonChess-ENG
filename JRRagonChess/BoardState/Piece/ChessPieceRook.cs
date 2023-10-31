using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceRook : ChessPieceBase {
        public ChessPieceRook(int team, Position position)
            : base(ChessPieceRookId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) =>
            GetSlidingMoves(currentBoardState, moveOffsets[4..]);



        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) =>
            base.IsMoveValid(move, currentBoardState) &&
            (new List<int>(moveOffsets[4..]).FindIndex(SlidingOffsetSelector(move)) != -1);
    }
}
