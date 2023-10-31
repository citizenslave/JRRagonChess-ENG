using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;



namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    internal class ChessPieceKing : ChessPieceBase {
        public ChessPieceKing(int team, Position position)
            : base(ChessPieceKingId, team, position) { }

        private static readonly int[] queensideCastleOffsets = new int[] { -1, -2, -3 },
            kingsideCastleOffsets = new int[] { 1, 2 };



        protected override List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) {
            List<ChessMove> legalMoves = GetFixedOffsetMoves(currentBoardState, moveOffsets);
            legalMoves.AddRange(GetCastleMoves(currentBoardState));

            return legalMoves;
        }

        private List<ChessMove> GetCastleMoves(Board currentBoardState) {
            List<ChessMove> castleMoves = new List<ChessMove>();

            if (currentBoardState.TeamHasCastleRights(chessTeam)) {
                castleMoves.AddRange(GetSideCastleMove(currentBoardState, chessTeam, true));
                castleMoves.AddRange(GetSideCastleMove(currentBoardState, chessTeam, false));
            }

            return castleMoves;
        }

        private List<ChessMove> GetSideCastleMove(Board board, ChessTeam team, bool isQueenside) {
            if (!board.GetCastleRights(team, isQueenside)) return new List<ChessMove>();
            foreach (int offset in isQueenside ? queensideCastleOffsets : kingsideCastleOffsets)
                if (board[piecePosition.OffsetByIndex(offset).Index] != ChessPieceNone) return new List<ChessMove>();

            return new List<ChessMove>() {
                new ChessMove(
                    piecePosition,
                    piecePosition.OffsetByIndex(2 * (isQueenside ? -1 : 1)),
                    ChessMove.MoveFlag.Castle
                )
            };
        }



        protected override bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (!base.IsMoveValid(move, currentBoardState)) return false;

            return true;
        }
    }
}
