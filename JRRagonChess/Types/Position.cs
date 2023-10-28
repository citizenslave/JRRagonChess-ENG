using JRRagonGames.JRRagonChess.BoardState;

namespace JRRagonGames.JRRagonChess.Types {
    public struct Position {
        public int rank, file;

        public readonly int Index {
            get {
                return GetIndex(rank, file);
            }
        }

        public readonly Position OffsetByIndex(int offsetIndex) => GetPositionFromIndex(Index + offsetIndex);

        public override readonly string ToString() => GetNameFromPosition(file, rank);

        public static int GetIndex(string squareName) => GetPositionFromName(squareName).Index;
        public static int GetIndex(int rank, int file) => rank * Board.Constants.FileCount + file;
        public static int GetFileFromName(string name) => "abcdefgh".IndexOf(char.ToLower(name[0]));
        public static int GetRankFromName(string name) => (int)(char.GetNumericValue(name[1]) - 1);
        public static int GetFileFromIndex(int index) => index % Board.Constants.FileCount;
        public static int GetRankFromIndex(int index) => index / Board.Constants.FileCount;
        public static string GetNameFromPosition(int file, int rank) => $"{"abcdefgh"[file]}{rank + 1}";
        public static Position GetPositionFromName(string name) =>
            new Position() { rank = GetRankFromName(name), file = GetFileFromName(name) };
        public static Position GetPositionFromIndex(int squareIndex) =>
            new Position() { rank = GetRankFromIndex(squareIndex), file = GetFileFromIndex(squareIndex) };
    }
}