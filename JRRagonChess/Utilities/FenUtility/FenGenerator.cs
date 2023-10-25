using JRRagonGames.JRRagonChess.BoardState;
using JRRagonGames.JRRagonChess.BoardState.Piece;

using static JRRagonGames.JRRagonChess.BoardState.Board.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.EnPassantUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.HalfTurnUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.TurnCountUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.ActiveTeamUtil.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.CastleRightsUtil.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.EnPassantUtil.Constants;

namespace JRRagonGames.JRRagonChess.Utilities {
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
                        positions += ChessPieceBase.GetFenCode(piece);
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
                (boardData.ActiveTeam != BlackTurn ? "w" : "b");

            private static string GetCastleRights(Board boardData) =>
                ((boardData.GameData & CastleMask) == 0 ? "-" :
                    ((boardData.GameData & WhiteKingsCastle) != 0 ? "K" : "") +
                    ((boardData.GameData & WhiteQueenCastle) != 0 ? "Q" : "") +
                    ((boardData.GameData & BlackKingsCastle) != 0 ? "k" : "") +
                    ((boardData.GameData & BlackQueenCastle) != 0 ? "q" : "") +
                "");

            private static string GetEnPassant(Board boardData) =>
                ((boardData.GameData & EnPassantMask) == 0 ? "-"
                : LookupEnPassant(boardData.GameData));

            private static string GetHalfTurns(Board boardData) =>
                UnloadHalfTurns(boardData.GameData);

            private static string GetTurnCount(Board boardData) =>
                UnloadTurnCount(boardData.GameData);
        }
    }
}