using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKnight : ChessPieceBase {
        public ChessPieceKnight(int team, Position position, Board board)
            : base(ChessPieceKnightId, team, position, board) { }

        private static new readonly int[] moveOffsets = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };



        protected override List<ChessMove> GetPseudoLegalMovesForPiece() =>
            GetFixedOffsetMoves(moveOffsets);



        protected override bool IsMoveValid(ChessMove move) {
            if (!base.IsMoveValid(move)) return false;

            if (!new List<int>(moveOffsets).Contains(move.EndPosition.Index - move.StartPosition.Index)) return false;

            return true;
        }
    }
}
