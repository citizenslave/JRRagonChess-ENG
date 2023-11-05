using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;



namespace JRRagonGames.JRRagonChess {
    public class MoveGenerator {
        public MoveGenerator(ChessGame _currentGame) { currentGame = _currentGame; }
        private readonly ChessGame currentGame;



        #region Move Generation
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
            ChessGame temporaryGame = currentGame.GetSimulation();
            temporaryGame.ExecuteMove(move, true);

            return !temporaryGame.IsInCheck(false);
        }
        #endregion



        #region Simulation Interface
        public bool IsPositionThreatened(Position piecePosition) =>
            GenerateAllMoves(false).FindIndex(move => move.EndPosition.Index == piecePosition.Index) != -1;

        public Dictionary<ChessMove,int> PERFTlist(int depth = 1) {
            Dictionary<ChessMove,int> results = new Dictionary<ChessMove,int>();

            foreach (ChessMove move in GenerateAllMoves(true)) {
                if (depth == 1) results[move] = 1;
                else {
                    ChessGame temporaryGame = currentGame.GetSimulation();
                    temporaryGame.ExecuteMove(move);
                    results[move] = new MoveGenerator(temporaryGame).PERFT(depth - 1);
                }
            }

            return results;
        }

        public int PERFT(int depth = 1) {
            if (depth == 1) return GenerateAllMoves(true).Count;
            else {
                int treeTotal = 0;
                foreach (ChessMove move in GenerateAllMoves(true)) {
                    ChessGame temporaryGame = currentGame.GetSimulation();
                    temporaryGame.ExecuteMove(move);
                    treeTotal += new MoveGenerator(temporaryGame).PERFT(depth - 1);
                }
                return treeTotal;
            }
        }
        #endregion
    }
}
