using System.Collections.Generic;
using JRRagonGames.JRRagonChess.BoardState;
using JRRagonGames.JRRagonChess.BoardState.Piece;
using JRRagonGames.JRRagonChess.Types;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess {
    public class MoveGenerator {
        public MoveGenerator(ChessGame _currentGame) { 
            currentGame = _currentGame;
        }

        private readonly ChessGame currentGame;

        public List<ChessMove> GenerateAllMoves(bool legal = true) {
            List<ChessMove> allMoves = new List<ChessMove>();
            Board currentBoard = currentGame.CurrentBoardState;

            int[] tiles = currentBoard.PieceDataBySquare;
            for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++) {

                int pieceNibble = tiles[tileIndex];
                bool hasPiece = pieceNibble != ChessPieceNone;

                int activeTeamPieces = currentBoard.ActiveTeamPiece;
                int pieceTeam = ChessPieceBase.GetPieceTeamRaw(pieceNibble);
                bool isOwnedPiece = pieceTeam == activeTeamPieces;

                bool canMoveFromPosition = hasPiece && isOwnedPiece;
                
                if (canMoveFromPosition)
                    allMoves.AddRange(
                        ChessPieceBase.GetPseudoLegalMovesFromPosition(
                            Position.GetPositionFromIndex(tileIndex),
                            currentBoard
                        ).FindAll(move => !legal || IsLegalMove(move))
                    );
            }

            return allMoves;
        }

        public bool IsLegalMove(ChessMove move) {
            ChessGame temporaryGame = new ChessGame(currentGame.CurrentBoardState.Copy(), true);
            temporaryGame.ExecuteMove(move, true);

            return !(new MoveGenerator(temporaryGame)).IsInCheck();
        }

        public bool IsInCheck() {
            List<ChessMove> allMoves = GenerateAllMoves(false);
            Position checkedKingPosition = currentGame.CurrentBoardState.OtherKingPosition;

            return allMoves.FindIndex(move => move.EndPosition.Index == checkedKingPosition.Index) != -1;
        }
    }
}
