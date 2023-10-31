using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceRook : ChessPieceBase {
        public ChessPieceRook(int team, Position position)
            : base(ChessPieceRookId, team, position) { }

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) =>
            GetSlidingMoves(currentBoardState, moveOffsets[4..]);



        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            if (new List<int>(moveOffsets[4..]).FindIndex(
                offset => (move.EndPosition.Index - move.StartPosition.Index) % offset == 0
            ) == -1) return false;

            return true;
        }
    }
}
