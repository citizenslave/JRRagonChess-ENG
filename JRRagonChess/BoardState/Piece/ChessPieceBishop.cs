using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceBishop : ChessPieceBase {
        public ChessPieceBishop(int team, Position position)
            : base(ChessPieceBishopId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) =>
            GetSlidingMoves(currentBoardState, moveOffsets[0..4]);



        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            return true;
        }
    }
}
