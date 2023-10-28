using JRRagonGames.JRRagonChess.BoardState;

namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static partial class FenUtility {
        public const string startpos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static string ExtractCurrentFen(Board boardData) => FenGenerator.GenerateFen(boardData);

        public static Board ParseFen(string fen) => FenParser.ParseFen(fen);
    }
}