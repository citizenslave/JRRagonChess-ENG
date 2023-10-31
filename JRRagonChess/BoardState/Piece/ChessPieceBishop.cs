using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceBishop : ChessPieceBase {
        public ChessPieceBishop(int team, Position position)
            : base(ChessPieceBishopId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) =>
            GetSlidingMoves(currentBoardState, moveOffsets[0..4]);



        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) =>
            base.IsMoveValid(move, currentBoardState) &&
            (new List<int>(moveOffsets[0..4]).FindIndex(SlidingOffsetSelector(move)) != -1);
    }
}
