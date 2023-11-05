using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceBishop : ChessPieceBase {
        public ChessPieceBishop(int team, Position position, Board board)
            : base(ChessPieceBishopId, team, position, board) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(bool quiet = true) =>
            GetSlidingMoves(moveOffsets[0..4]);



        protected override bool IsMoveValid(ChessMove move) =>
            base.IsMoveValid(move) &&
            (new List<int>(moveOffsets[0..4]).FindIndex(SlidingOffsetSelector(move)) != -1);
    }
}
