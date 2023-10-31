using JRRagonGames.JRRagonChess.BoardState;
using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.Types.BoardConstants;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;



namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static partial class FenUtility {
        private static class FenParser {
            public static Board ParseFen(string fen) {
                const byte pieceIndex = 0;
                const byte activeTeamIndex = 1;
                const byte castleRightsIndex = 2;
                const byte enPassantIndex = 3;
                const byte halfTurnIndex = 4;
                const byte turnCountIndex = 5;

                string[] fenParts = fen.Split(' ');



                int[] piecesOnSquares = GetPiecesOnSquares(fenParts[pieceIndex]);

                ChessTeam activeTeamData = GetActiveTeam(fenParts[activeTeamIndex]);
                CastleRights castleRightsData = GetCastleRights(fenParts[castleRightsIndex]);
                string enPassantData = GetEnPassantFile(fenParts[enPassantIndex]);
                int halfTurnData = GetHalfTurns(fenParts[halfTurnIndex]);
                int turnCountData = GetTurnCount(fenParts[turnCountIndex]);

                Board data = new Board(
                    piecesOnSquares,
                    activeTeamData,
                    castleRightsData,
                    enPassantData,
                    halfTurnData,
                    turnCountData
                );



                return data;
            }



            private static int[] GetPiecesOnSquares(string piecePositionData) {
                int[] piecesOnSquares = new int[TileCount];



                int rankIndex = FileCount - 1, fileIndex = 0;
                foreach (char piece in piecePositionData) {
                    if (piece == '/') { fileIndex = 0; rankIndex--; continue; }
                    if (char.IsDigit(piece)) { fileIndex += (int)char.GetNumericValue(piece); continue; }

                    piecesOnSquares[Position.GetIndex(rankIndex, fileIndex++)] = GetNibbleFromFen(piece);
                }



                return piecesOnSquares;
            }



            private static ChessTeam GetActiveTeam(string activeTeamFenData) =>
                (activeTeamFenData == "w" ? ChessTeam.WhiteTeam : ChessTeam.BlackTeam);

            private static CastleRights GetCastleRights(string castleFenData) =>
                castleFenData == "-" ? new CastleRights(false, false, false, false)
                    : new CastleRights(
                        castleFenData.Contains("K"),
                        castleFenData.Contains("Q"),
                        castleFenData.Contains("k"),
                        castleFenData.Contains("q")
                    );

            private static string GetEnPassantFile(string enPassantData) =>
                enPassantData == "-" ? "" : enPassantData;

            private static int GetHalfTurns(string halfTurnData) =>
                int.Parse(halfTurnData);

            private static int GetTurnCount(string turnCountData) =>
                int.Parse(turnCountData);
        }
    }
}
