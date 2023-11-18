using static JRRagonGames.JRRagonChess.Types.BoardConstants;

namespace JRRagonGames.JRRagonChess.Types {
    public readonly struct Position {
        #region Data/Constructor
        private static readonly int WhiteSquare = 0x1;
        private static readonly int BlackSquare = 0x2;

        public readonly int rank, file;
        public Position(int _rank, int _file) { rank = _rank; file = _file; }
        #endregion



        #region Instance Accessors
        public readonly int Index => GetIndex(rank, file);
        public override readonly string ToString() => GetNameFromPosition(file, rank);



        public readonly Position OffsetByIndex(int offsetIndex) => GetPositionFromIndex(Index + offsetIndex);
        #endregion






        #region Static Utilities



        #region Component Extraction
        #region Components from Index
        public static int GetFileFromIndex(int index) => index % BoardConstants.FileCount;
        public static int GetRankFromIndex(int index) => index / BoardConstants.FileCount;
        #endregion



        #region Components from String
        public static int GetFileFromName(string name) =>
            !string.IsNullOrEmpty(name) ? "abcdefgh".IndexOf(char.ToLower(name[0])) : -1;
        public static int GetRankFromName(string name) =>
            !string.IsNullOrEmpty(name) && name.Length > 1 ? (int)(char.GetNumericValue(name[1]) - 1) : -1;
        #endregion
        #endregion



        #region Conversion Utilities
        #region Index Conversion
        public static int GetIndex(string squareName) => GetPositionFromName(squareName).Index;
        public static int GetIndex(int rank, int file) => rank * FileCount + file;
        public static int GetColorFlag(int index) => 
            (index % FileCount) % 2 == ((index / FileCount) % 2 == 0 ? 1 : 0) ? WhiteSquare : BlackSquare;
        #endregion



        #region Name Conversion
        public static string GetNameFromPosition(int file, int rank) => $"{"abcdefgh"[file]}{rank + 1}";
        #endregion
        #endregion



        #region Factories
        public static Position GetPositionFromName(string name) =>
            new Position(GetRankFromName(name), GetFileFromName(name));
        public static Position GetPositionFromIndex(int squareIndex) =>
            new Position(GetRankFromIndex(squareIndex), GetFileFromIndex(squareIndex));
        #endregion


        #region Operators
        public static bool operator ==(Position p1, Position p2) => p1.Index == p2.Index;
        public static bool operator !=(Position p1, Position p2) => p1.Index != p2.Index;
        public override bool Equals(object obj) => obj is Position position && Equals(position);
        public override int GetHashCode() => base.GetHashCode();
        #endregion



        #endregion



    }
}
