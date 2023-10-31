using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;

namespace JRRagonGames.JRRagonChess {
    public class MoveGenerator {
        public MoveGenerator(ChessGame _currentGame) { currentGame = _currentGame; }

        private readonly ChessGame currentGame;

        public List<ChessMove> GenerateAllMoves(bool legal = true) {
            List<ChessMove> allMoves = new List<ChessMove>();

            for (int tileIndex = 0; tileIndex < BoardConstants.TileCount; tileIndex++) {
                int pieceNibble = currentGame[tileIndex];

                bool hasPiece = pieceNibble != PieceNone;
                if (!hasPiece) continue;

                ChessTeam activeTeam = currentGame.ActiveTeam;
                ChessTeam pieceTeam = ExtractTeamFromNibble(pieceNibble);
                bool isOwnedPiece = pieceTeam == activeTeam;
                if (!isOwnedPiece) continue;

                Position ownedPiecePosition = Position.GetPositionFromIndex(tileIndex);
                List<ChessMove> ownedPiecePseudoLegalMoves = currentGame.GetPseudoLegalMovesFrom(ownedPiecePosition);

                allMoves.AddRange(ownedPiecePseudoLegalMoves);
            }

            if (legal) allMoves = allMoves.FindAll(IsLegalMove);

            return allMoves;
        }

        private bool IsLegalMove(ChessMove move) {
            ChessGame temporaryGame = new ChessGame(currentGame.CurrentBoardState.Copy(), true);
            temporaryGame.ExecuteMove(move, true);

            return !(new MoveGenerator(temporaryGame)).IsInCheck();
        }

        public bool IsInCheck() {
            Position piecePosition = currentGame.CurrentBoardState.OtherKingPosition;

            return GenerateAllMoves(false).FindIndex(move => move.EndPosition.Index == piecePosition.Index) != -1;
        }
    }
}
