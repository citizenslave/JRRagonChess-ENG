using System;
using System.Collections.Generic;

using JRRagonGames.Utilities;

using JRRagonGames.JRRagonChess.Types;
using JRRagonGames.JRRagonChess.BoardState.Piece;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;

namespace JRRagonGames.JRRagonChess.BoardState {
    public partial class Board {



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

        public int GameData { get => gameDataRegister.BitData; private set => gameDataRegister.BitData = value; }
        private readonly BitRegister gameDataRegister = new BitRegister(0);
        #endregion



        #region Constructors & Factories
        public Board(int[] _pieceData, int _GameData) { pieceData = _pieceData; gameDataRegister.BitData = _GameData; }

        public Board Copy() => new Board((int[])pieceData.Clone(), gameDataRegister.BitData);

        public Board(
            int[] _pieceData,
            ChessTeam _activeTeam,
            CastleRights _castleRights,
            string _enPassantFileName,
            int _halfCount,
            int _turnCount
        ) {
            pieceData = (int[])_pieceData.Clone();

            AllCastleRights = _castleRights;
            ActiveChessTeam = _activeTeam;
            if (!string.IsNullOrEmpty(_enPassantFileName)) EnPassantName = _enPassantFileName;
            HalfCount = _halfCount;
            TurnCount = _turnCount;
        }
        #endregion



        #region Piece Data
        public int this[int squareIndex] {
            get => pieceData[squareIndex];
            set => pieceData[squareIndex] = value;
        }
        private readonly int[] pieceData;

        #region King Finder
        public Position ActiveKingPosition => FindKing(ActiveChessTeam);
        public Position OtherKingPosition => FindKing(OtherChessTeam);

        private Position FindKing(ChessTeam team) => Position.GetPositionFromIndex(
            new List<int>(pieceData).FindIndex(
                pieceAtIndex => ChessPieceBase.GetPieceNibble(team, PieceKing) == pieceAtIndex
            )
        );
        #endregion



        #region Move Generation and Validation
        public List<ChessMove> GetPseudoLegalMovesFrom(Position startPosition) =>
            ChessPieceBase.GetPseudoLegalMovesFromPosition(startPosition, this);

        public bool IsMoveValid(ChessMove move) => ChessPieceBase.IsMovePossible(move, this);
        #endregion
        #endregion



        public override string ToString() {
            byte[] _gameData = BitConverter.GetBytes(gameDataRegister.BitData);
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
