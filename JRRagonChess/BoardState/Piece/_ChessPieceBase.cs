using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;

using static JRRagonGames.JRRagonChess.BoardState.Piece.ChessPieceBase.Constants;

namespace JRRagonGames.JRRagonChess.BoardState.Piece {
    public partial class ChessPieceBase {



        protected int pieceTypeNibble;
        protected int pieceTeamNibble;
        protected int teamIndex;
        protected ChessTeam chessTeam;
        protected Position piecePosition;



        public ChessPieceBase(int type, int team, Position position) {
            pieceTypeNibble = type;
            pieceTeamNibble = team;
            teamIndex = pieceTeamNibble >> TeamIndexOffset;
            chessTeam = (ChessTeam)teamIndex;
            piecePosition = position;
        }



        protected static readonly int[] moveOffsets = new int[] { -9, -7, 7, 9, -8, -1, 1, 8 };



        public static List<ChessMove> GetPseudoLegalMovesFromPosition(Position startPosition, Board currentBoardState) =>
            PieceFactory(startPosition, currentBoardState).GetPseudoLegalMovesForPiece(currentBoardState);
        protected virtual List<ChessMove> GetPseudoLegalMovesForPiece(Board currentBoardState) => new List<ChessMove>();

        protected List<ChessMove> GetFixedOffsetMoves(
            Board currentBoardState,
            int[] offsets,
            int directionMultiplier = 1
        ) => new List<int>(offsets).FindAll(o => IsValidSquare(currentBoardState, this, o * directionMultiplier))
                .ConvertAll(o => new ChessMove(piecePosition, piecePosition.OffsetByIndex(o * directionMultiplier)));

        protected List<ChessMove> GetSlidingMoves(Board currentBoardState, int[] moveOffsets) {
            List<ChessMove> moves = new List<ChessMove>();

            foreach (int moveOffset in moveOffsets) {
                for (int searchIndex = piecePosition.Index; IsValidSquare(searchIndex, moveOffset); searchIndex += moveOffset) {
                    int targetIndex = searchIndex + moveOffset,
                        pieceNibbleAtTarget = currentBoardState[targetIndex];

                    bool targetingPiece = pieceNibbleAtTarget != ChessPieceNone,
                        targetingOpponent = targetingPiece && GetTeamFromNibble(pieceNibbleAtTarget) != chessTeam;

                    if (!targetingPiece || targetingOpponent)
                        moves.Add(new ChessMove(piecePosition, Position.GetPositionFromIndex(targetIndex)));
                    if (targetingPiece) break;
                }
            }

            return moves;
        }



        protected static bool IsValidSquare(Board currentBoardState, ChessPieceBase piece, int moveOffset, int max = 2) {
            if (!IsValidSquare(piece.piecePosition.Index, moveOffset, max)) return false;

            Position targetPosition = piece.piecePosition.OffsetByIndex(moveOffset);
            int pieceNibbleAtTarget = currentBoardState[targetPosition.Index];

            bool targetingPiece = pieceNibbleAtTarget != ChessPieceNone,
                targetingOpponent = targetingPiece && GetTeamFromNibble(pieceNibbleAtTarget) != piece.chessTeam;
            if (targetingPiece && !targetingOpponent) return false;

            return true;

        }

        protected static bool IsValidSquare(int initialIndex, int moveOffset, int max = 2) {
            int targetIndex = initialIndex + moveOffset;
            if (targetIndex < 0 || targetIndex >= Board.Constants.TileCount) return false;

            Position targetPosition = Position.GetPositionFromIndex(targetIndex);
            int toFile = targetPosition.file, fromFile = Position.GetFileFromIndex(initialIndex);
            if (Math.Abs(toFile - fromFile) > max) return false;

            return true;
        }



        protected virtual bool IsMoveValid(ChessMove move, Board currentBoardState) {
            if (pieceTypeNibble == ChessPieceNone) return false;
            
            int teamValidationPiece = GetPieceNibble(currentBoardState.ActiveChessTeam, pieceTypeNibble);
            if ((pieceTypeNibble | pieceTeamNibble) != teamValidationPiece) return false;
            
            int pieceToCapture = currentBoardState[move.EndPosition.Index];
            int endSquareTeam = GetPieceTeamRaw(pieceToCapture);
            if (pieceToCapture > 0 && pieceTeamNibble == endSquareTeam) return false;

            return true;
        }



        public static bool IsMovePossible(ChessMove move, Board currentBoardState) =>
            currentBoardState[move.StartPosition.Index] != ChessPieceNone &&
            PieceFactory(move.StartPosition, currentBoardState).IsMoveValid(move, currentBoardState);



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