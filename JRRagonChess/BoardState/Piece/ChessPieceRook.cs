using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceRook : ChessPieceBase {
        public ChessPieceRook(int team, Position position, Board board)
            : base(ChessPieceRookId, team, position, board) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(bool quiet = true) =>
            GetSlidingMoves(moveOffsets[4..]);



        protected override bool IsMoveValid(ChessMove move) =>
            base.IsMoveValid(move) &&
            (new List<int>(moveOffsets[4..]).FindIndex(SlidingOffsetSelector(move)) != -1);
    }
}
