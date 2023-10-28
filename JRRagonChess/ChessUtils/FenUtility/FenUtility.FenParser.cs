using JRRagonGames.JRRagonChess.BoardState;
using JRRagonGames.JRRagonChess.BoardState.Piece;

using static JRRagonGames.JRRagonChess.BoardState.Board.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.EnPassantUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.HalfTurnUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.TurnCountUtil;
using static JRRagonGames.JRRagonChess.BoardState.Board.ActiveTeamUtil.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.CastleRightsUtil.Constants;
using static JRRagonGames.JRRagonChess.BoardState.Board.EnPassantUtil.Constants;
using JRRagonGames.JRRagonChess.Types;

namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static partial class FenUtility {
        private static class FenParser {
            private static class FenIndex {
                public const byte PieceData = 0;
                public const byte ActiveTeamData = 1;
                public const byte CastleData = 2;
                public const byte EnPassantData = 3;
                public const byte HalfTurnData = 4;
                public const byte TurnCountData = 5;
            }

            public static Board ParseFen(string fen) {
                string[] fenParts = fen.Split(' ');

                int[] piecesOnSquares = GetPiecesOnSquares(fenParts[FenIndex.PieceData]);

                int activeTeamData = GetActiveTeam(fenParts[FenIndex.ActiveTeamData]);
                int castleRightsData = GetCastleRights(fenParts[FenIndex.CastleData]);
                int enPassantData = GetEnPassantFile(fenParts[FenIndex.EnPassantData]);
                int halfTurnData = GetHalfTurns(fenParts[FenIndex.HalfTurnData]);
                int turnCountData = GetTurnCount(fenParts[FenIndex.TurnCountData]);

                int gameData =
                    activeTeamData |
                    castleRightsData |
                    enPassantData |
                    halfTurnData |
                    turnCountData |
                0;

                Board data = new Board(piecesOnSquares, gameData);

                return data;
            }

            private static int[] GetPiecesOnSquares(string piecePositionData) {
                int[] piecesOnSquares = new int[TileCount];

                int rankIndex = FileCount - 1, fileIndex = 0;
                foreach (char piece in piecePositionData) {
                    if (piece == '/') { fileIndex = 0; rankIndex--; continue; }
                    if (char.IsDigit(piece)) { fileIndex += (int)char.GetNumericValue(piece); continue; }

                    piecesOnSquares[Position.GetIndex(rankIndex, fileIndex++)] = ChessPieceBase.GetPieceNibble(piece);
                }

                return piecesOnSquares;
            }

            private static int GetActiveTeam(string activeTeamFenData) =>
                (activeTeamFenData == "w" ? WhiteTurn : BlackTurn);

            private static int GetCastleRights(string castleFenData) =>
                castleFenData == "-" ? 0
                    : 0 |
                        (castleFenData.Contains("K") ? WhiteKingsCastle : 0) |
                        (castleFenData.Contains("Q") ? WhiteQueenCastle : 0) |
                        (castleFenData.Contains("k") ? BlackKingsCastle : 0) |
                        (castleFenData.Contains("q") ? BlackQueenCastle : 0) |
                    0;

            private static int GetEnPassantFile(string enPassantData) =>
                enPassantData == "-" ? NoEnPassant : GetEnPassant(enPassantData);

            private static int GetHalfTurns(string halfTurnData) =>
                PadHalfTurn(int.Parse(halfTurnData));

            private static int GetTurnCount(string turnCountData) =>
                PadTurnCount(int.Parse(turnCountData));
        }
    }
}