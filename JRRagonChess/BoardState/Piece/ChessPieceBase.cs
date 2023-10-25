using System;
using System.Collections.Generic;
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

        protected virtual List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) => new List<ChessMove>();

        public static List<ChessMove> GetPseudoLegalMovesFromPosition(Position startPosition, Board currentBoardState) =>
            PieceFactory(startPosition, currentBoardState).GetPseudoLegalMovesForPiece(currentBoardState);

        protected static bool IsValidSquare(int initialIndex, int moveOffset) {
            int targetIndex = initialIndex + moveOffset;
            if (targetIndex < 0 || targetIndex >= Board.Constants.TileCount) return false;

            Position targetPosition = Position.GetPositionFromIndex(targetIndex);
            int toFile = targetPosition.file, fromFile = Position.GetFileFromIndex(initialIndex);
            if (Math.Abs(toFile - fromFile) > 2) return false;

            return true;
        }

        public static bool IsMoveValid(ChessMove move, Board currentBoardState) {
            return PieceFactory(move.StartPosition, currentBoardState).IsMoveLegal(move, currentBoardState);
        }

        protected virtual bool IsMoveLegal(ChessMove move, Board currentBoardState) {
            if (PieceType == ChessPieceNone) return false;
            
            int teamValidationPiece = GetPieceNibble(currentBoardState.ActiveTeam, PieceType);
            if ((PieceType | PieceTeam) != teamValidationPiece) return false;
            
            int pieceToCapture = currentBoardState[move.EndPosition.Index];
            int endSquareTeam = GetPieceTeam(pieceToCapture);
            if (pieceToCapture > 0 && PieceTeam == endSquareTeam) return false;

            return true;
        }

        protected static bool TryAddMove(ChessMove move, Board currentBoard, List<ChessMove> moves) {
            moves.Add(move);
            return true;
        }

        private static ChessPieceBase PieceFactory(Position startPosition, Board currentBoardState) {
            int pieceToMove = currentBoardState[startPosition.Index];
            int pieceTeam = GetPieceTeam(pieceToMove);
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

        public static int GetPieceNibble(int activeTeam, int pieceType) => pieceType | (activeTeam >> TeamPieceOffset);

        public static char GetFenCode(int piece) => FenIndex[piece];

        public static int GetPieceTeam(int piece) => piece & ChessPieceTeamMask;

        public static int GetPieceType(int piece) => piece & ChessPieceTypeMask;
    }
}