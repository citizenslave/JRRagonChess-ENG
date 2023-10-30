using JRRagonGames.JRRagonChess.BoardState;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;
using static JRRagonGames.JRRagonChess.Types.BoardConstants;

namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static partial class FenUtility {
        private static class FenGenerator {
            public static string GenerateFen(Board boardData) {
                string fen = "" +
                    $"{GetPiecePositions(boardData)} " +
                    $"{GetActiveTeam(boardData)} " +
                    $"{GetCastleRights(boardData)} " +
                    $"{GetEnPassant(boardData)} " +
                    $"{GetHalfTurns(boardData)} " +
                    $"{GetTurnCount(boardData)}" +
                "";

                return fen;
            }

            private static string GetPiecePositions(Board boardData) {
                string positions = "";



                int rankIndex = FileCount - 1, fileIndex = 0, spaceCounter = 0;
                while (rankIndex >= 0) {



                    int piece = boardData[Position.GetIndex(rankIndex, fileIndex++)];



                    if (piece == 0) spaceCounter++;
                    else {
                        if (spaceCounter > 0) { positions += spaceCounter; spaceCounter = 0; }
                        positions += GetFenCharFromNibble(piece);
                    }



                    if (fileIndex % FileCount == 0) {
                        rankIndex--;
                        positions += $"{(spaceCounter > 0 ? spaceCounter.ToString() : "")}/";
                        spaceCounter = fileIndex = 0;
                    }
                }

                return positions[..^1];
            }

            private static string GetActiveTeam(Board boardData) =>
                boardData.ActiveChessTeam != ChessTeam.BlackTeam ? "w" : "b";

            private static string GetCastleRights(Board boardData) =>
                !boardData.HasCastleRights ? "-" :
                    (boardData.CastleRightsWhiteKing ? "K" : "") +
                    (boardData.CastleRightsWhiteQueen ? "Q" : "") +
                    (boardData.CastleRightsBlackKing ? "k" : "") +
                    (boardData.CastleRightsBlackQueen ? "q" : "") +
                "";

            private static string GetEnPassant(Board boardData) =>
                !boardData.HasEnPassant ? "-" : boardData.EnPassantName;

            private static string GetHalfTurns(Board boardData) =>
                boardData.HalfCount.ToString();

            private static string GetTurnCount(Board boardData) =>
                boardData.TurnCount.ToString();
        }
    }
}
