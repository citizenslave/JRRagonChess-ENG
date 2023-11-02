using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceQueen : ChessPieceBase {
        public ChessPieceQueen(int team, Position position, Board board)
            : base(ChessPieceQueenId, team, position, board) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece() =>
            GetSlidingMoves(moveOffsets);



        protected override bool IsMoveValid(ChessMove move) =>
            base.IsMoveValid(move) &&
            (new List<int>(moveOffsets).FindIndex(SlidingOffsetSelector(move)) != -1);
    }
}
