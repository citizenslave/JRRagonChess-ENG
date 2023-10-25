using System.Collections.Generic;
using JRRagonGames.JRRagonChess.BoardState.Piece;
using System;
using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {
        public static class Constants {
            public const int TileCount = 64;
            public const int FileCount = 8;
        }

        #region GameData
        /** GameData Registers:
         * The Utility classes under the BoardState address a single GameData int
         * using the following registers to store all data related to the FEN.
         * 
         * 0000 0000 0000 0000 0000 0000 0000 0000
         * r|-open-| |c-| |ep| t|--ht--| |--turn-|
         * 
         * r    :reserved. setting this bit in a literal becomes a uint requiring 
         *       an unchecked casting.  just don't.
         * open :these bits are unused at this time.
         * c    :these bits store castling rights
         * ep   :first bit signals en passant, last 3 signal the affected file
         * t    :active team, true for black
         * ht   :half turn counter
         * turn :turn counter
         */
        public int GameData { get; private set; }
        #endregion

        #region Constructors & Factories
        public Board(int[] _pieceData, int _GameData) { pieceData = _pieceData; GameData = _GameData; }

        public Board(
            int[] _pieceData,
            int _activeTeam,
            int _castleRights,
            int _enPassant,
            int _halfTurn,
            int _turnCount
        ) { 
            pieceData = (int[])_pieceData.Clone();
            GameData = 0 |
                _activeTeam |
                _castleRights |
                _enPassant |
            0;

            HalfTurn = _halfTurn;
            TurnCount = _turnCount;
        }

        public Board Copy() => new Board((int[])pieceData.Clone(), GameData);
        #endregion

        #region Piece Data
        public int this[int squareIndex] {
            get => pieceData[squareIndex];
            set => pieceData[squareIndex] = value;
        }
        public int[] PieceDataBySquare { get => (int[])pieceData.Clone(); }
        private readonly int[] pieceData;

        public Position ActiveKingPosition =>
            FindPiece(ActiveTeam, ChessPieceKingId);

        public Position OtherKingPosition =>
            FindPiece(OtherTeam, ChessPieceKingId);

        private Position FindPiece(int pieceTeam, int pieceType) =>
            Position.GetPositionFromIndex(
                new List<int>(pieceData).FindIndex(x =>
                    x == ChessPieceBase.GetPieceNibble(pieceTeam, pieceType)
                )
            );
        #endregion

        public override string ToString() {
            byte[] _gameData = BitConverter.GetBytes(GameData);
            Array.Reverse(_gameData);
            string boardBottom = $" ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯\n  A B C D E F G H\n   {BitConverter.ToString(_gameData)}",
                boardView = $"  A B C D E F G H\n _________________\n{Constants.FileCount}";

            int rankIndex = Constants.FileCount - 1, fileIndex = 0;
            while (rankIndex >= 0) {
                int squareIndex = Position.GetIndex(rankIndex, fileIndex++);
                boardView += $"|{ChessPieceBase.GetFenCode(pieceData[squareIndex])}";
                if (fileIndex % Constants.FileCount == 0 ) {
                    fileIndex = 0;
                    rankIndex--;
                    boardView += $"|{rankIndex + 2}\n{rankIndex + 1}";
                }
            }

            return boardView[0..^1] + boardBottom;
        }
    }
}