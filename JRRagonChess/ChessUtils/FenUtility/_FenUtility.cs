using JRRagonGames.JRRagonChess.BoardState;



namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static partial class FenUtility {
        public const string startpos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";



        public static Board ParseFen(string fen) => FenParser.ParseFen(fen);
        public static Board DeserializeBoard(byte[] boardBytes) => BoardSerializer.DeserializeBoard(boardBytes);



        public static byte[] SerializeBoard(Board boardState) => BoardSerializer.SerializeBoard(boardState);
        public static byte[] SerializeFen(string fen) => SerializeBoard(ParseFen(fen));



        public static string ExtractCurrentFen(Board boardData) => FenGenerator.GenerateFen(boardData);
        public static string DeserializeFen(byte[] boardBytes) => ExtractCurrentFen(DeserializeBoard(boardBytes));
    }
}
