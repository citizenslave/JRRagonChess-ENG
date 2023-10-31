namespace JRRagonGames.JRRagonChess.Types {
    public readonly struct Position {
        #region Data/Constructor
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
        public static int GetFileFromName(string name) => "abcdefgh".IndexOf(char.ToLower(name[0]));
        public static int GetRankFromName(string name) => (int)(char.GetNumericValue(name[1]) - 1);
        #endregion
        #endregion



        #region Conversion Utilities
        #region Index Conversion
        public static int GetIndex(string squareName) => GetPositionFromName(squareName).Index;
        public static int GetIndex(int rank, int file) => rank * BoardConstants.FileCount + file;
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



        #endregion



    }
}
