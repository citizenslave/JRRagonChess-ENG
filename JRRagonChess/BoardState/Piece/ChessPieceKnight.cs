using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKnight : ChessPieceBase {
        public ChessPieceKnight(int team, Position position)
            : base(ChessPieceKnightId, team, position) { }

        private new readonly int[] moveOffsets = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };
        /// Matching capture offsets, home ranks irrelevant

        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) =>
            GetFixedOffsetMoves(currentBoardState, moveOffsets);

        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            if (!new List<int>(moveOffsets).Contains(move.EndPosition.Index - move.StartPosition.Index)) return false;

            return true;
        }
    }
}
