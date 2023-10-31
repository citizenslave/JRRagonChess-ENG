using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    public partial class ChessPieceBase {



        protected int PieceType;
        protected int PieceTeam;
        protected int TeamIndex;
        protected Position PiecePosition;



        public ChessPieceBase(int type, int team, Position position) {
            PieceType = type;
            PieceTeam = team;
            TeamIndex = PieceTeam >> TeamIndexOffset;
            PiecePosition = position;
        }



        protected readonly int[] moveOffsets = new int[] { -9, -7, 7, 9, -8, -1, 1, 8 };

        public static List<ChessMove> GetPseudoLegalMovesFromPosition(Position startPosition, Board currentBoardState) =>
            PieceFactory(startPosition, currentBoardState).GetPseudoLegalMovesForPiece(currentBoardState);
        protected virtual List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) => new List<ChessMove>();



        protected static bool IsValidSquare(int initialIndex, int moveOffset, int max = 2) {
            int targetIndex = initialIndex + moveOffset;
            if (targetIndex < 0 || targetIndex >= Board.Constants.TileCount) return false;

            Position targetPosition = Position.GetPositionFromIndex(targetIndex);
            int toFile = targetPosition.file, fromFile = Position.GetFileFromIndex(initialIndex);
            if (Math.Abs(toFile - fromFile) > max) return false;

            return true;
        }



        protected virtual bool IsMoveLegal(ChessMove move, Board currentBoardState) {
            if (PieceType == ChessPieceNone) return false;
            
            int teamValidationPiece = GetPieceNibble(currentBoardState.ActiveChessTeam, PieceType);
            if ((PieceType | PieceTeam) != teamValidationPiece) return false;
            
            int pieceToCapture = currentBoardState[move.EndPosition.Index];
            int endSquareTeam = GetPieceTeamRaw(pieceToCapture);
            if (pieceToCapture > 0 && PieceTeam == endSquareTeam) return false;

            return true;
        }



        public static bool IsMovePossible(ChessMove move, Board currentBoardState) =>
            return PieceFactory(move.StartPosition, currentBoardState).IsMoveLegal(move, currentBoardState);
        }



        private static ChessPieceBase PieceFactory(Position startPosition, Board currentBoardState) {
            int pieceToMove = currentBoardState[startPosition.Index];
            int pieceTeam = GetPieceTeamRaw(pieceToMove);
            int pieceType = GetPieceType(pieceToMove);
            
            return pieceType switch {
                ChessPiecePawnId => new ChessPiecePawn(pieceTeam, startPosition),
                ChessPieceKnightId => new ChessPieceKnight(pieceTeam, startPosition),
                ChessPieceKingId => new ChessPieceKing(pieceTeam, startPosition),
                ChessPieceRookId => new ChessPieceRook(pieceTeam, startPosition),
                ChessPieceBishopId => new ChessPieceBishop(pieceTeam, startPosition),
                ChessPieceQueenId => new ChessPieceQueen(pieceTeam, startPosition),
                _ => new ChessPieceBase(pieceType, pieceTeam, startPosition),
            };
        }



        public static int GetPieceNibble(char pieceFenCode) => FenIndex.IndexOf(pieceFenCode);

        public static int GetPieceNibble(ChessTeam team, int pieceType) => pieceType | ((int)team << TeamIndexOffset);



        public static char GetFenCode(int piece) => FenIndex[piece];







        public static int GetPieceTeamRaw(int piece) => piece & ChessPieceTeamMask;

        public static ChessTeam GetTeamFromNibble(int nibble) => (ChessTeam)(GetPieceTeamRaw(nibble) >> TeamIndexOffset);



        public static int GetPieceType(int piece) => piece & ChessPieceTypeMask;
    }
}