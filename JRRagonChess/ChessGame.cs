using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;
using JRRagonGames.JRRagonChess.BoardState;

using static JRRagonGames.JRRagonChess.ChessUtils.FenUtility;

using static JRRagonGames.JRRagonChess.Types.Position;
using static JRRagonGames.JRRagonChess.Types.BoardConstants;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;

namespace JRRagonGames.JRRagonChess {
    public class ChessGame {
        public int this[int boardTileIndex] => CurrentBoardState[boardTileIndex];

        public GameState CurrentGameState { get; private set; }

        public ChessTeam ActiveTeam => CurrentBoardState.ActiveChessTeam;
        public ChessTeam OtherTeam => CurrentBoardState.OtherChessTeam;

        public string FenCode => ExtractCurrentFen(CurrentBoardState);

        public Board CurrentBoardState { get; private set; }

        #region Move List
        public IReadOnlyList<ChessMove> MoveList => moveList.AsReadOnly();
        private readonly List<ChessMove> moveList = new List<ChessMove>();

        public List<ChessMove> GetAllLegalMoves() => new MoveGenerator(this).GenerateAllMoves();
        public List<ChessMove> GetLegalMovesFrom(Position position) => new List<ChessMove>();
        public List<ChessMove> GetPseudoLegalMovesFrom(Position position) => CurrentBoardState.GetPseudoLegalMovesFrom(position);
        #endregion

        #region Constructors
        public ChessGame() : this(ParseFen(startpos)) { }

        public ChessGame(string fenCode) : this(ParseFen(fenCode)) { }

        public ChessGame(Board boardState, bool isSimulated = false) {
            SetBoardState(CurrentBoardState = boardState);
            if (!isSimulated) UpdateGameState();
        }
        #endregion

        public void SetBoardState(Board state) {
            moveList.Clear();
            CurrentBoardState = state;
        }



        public int CapturedPieceIndex(ChessMove move) => move.Flag switch {




            /// TODO: Move this into the Board to remove the direct reference to the ActiveTeamUtil function.
            /// 

            ChessMove.MoveFlag.EnPassant => move.EndPosition.Index - (Board.TeamDirectionMultiplier(OtherTeam) * FileCount),




            _ => move.EndPosition.Index
        };



        public void ExecuteMove(ChessMove move, bool simulated = false) {
            int fromIndex = move.StartPosition.Index;
            int toIndex = move.EndPosition.Index;
            
            if (!CurrentBoardState.IsMoveValid(move)) return;
            
            int pieceToMove = CurrentBoardState[fromIndex];
            int flag = DetectMoveFlags(move);

            if (IsHalfTurnReset(pieceToMove, CurrentBoardState[toIndex])) CurrentBoardState.HalfCount = 0;
            else CurrentBoardState.HalfCount++;

            CurrentBoardState[fromIndex] = 0;
            CurrentBoardState[toIndex] = pieceToMove;
            moveList.Add(move);

            CurrentBoardState.EnPassant = 0;
            UpdateCastleRights(fromIndex, toIndex);
            ProcessMoveFlags(toIndex, flag);

            if (CurrentBoardState.ActiveChessTeam == ChessTeam.BlackTeam) CurrentBoardState.TurnCount++;
            CurrentBoardState.ActiveChessTeam = CurrentBoardState.OtherChessTeam;

            if (!simulated) UpdateGameState();
        }

        private void UpdateGameState() {
            MoveGenerator endGameSimulator = new MoveGenerator(this);
            if (endGameSimulator.GenerateAllMoves().Count == 0) EndGame(IsInCheck());






            /// TODO: Implement this properly.  The flag will immediately end the game and the straight counter
            /// overflows at 127, 23 plys before the progress draw is arbitrated.  The HT counter needs at least
            /// one more bit (which could be used to represent the flag in the game data)
            else if (CurrentBoardState.HalfCount > 100) CurrentGameState = GameState.Progress;
            //else if (CurrentBoardState.HalfTurn < 100 && CurrentGameState == GameState.Progress) CurrentGameState = GameState.Running;










            else {

                /// TODO: Implement more of this.  Two kings is the obvious material draw, but the minor piece cases
                /// need to be handled as well and this is not doing that.
                int pieceCounter = 0;
                for (int tileIndex = 0; tileIndex < TileCount; tileIndex++) {
                    if (CurrentBoardState[tileIndex] != PieceNone) pieceCounter++;
                }
                if (pieceCounter == 2) CurrentGameState = GameState.Material;






                /// TODO: 3-5 Fold Repetition Draw Condition
                /// 

            }


        }

        private void EndGame(bool isCheckMate) {
            CurrentGameState = isCheckMate ? GameState.Checkmate : GameState.Stalemate;
        }

        public bool IsInCheck() {
            Board flipTurnBoard = CurrentBoardState.Copy();
            flipTurnBoard.ActiveChessTeam = flipTurnBoard.OtherChessTeam;
            MoveGenerator endGameSimulator = new MoveGenerator(new ChessGame(flipTurnBoard));
            return endGameSimulator.IsInCheck();
        }

        #region MoveFlag Detection
        private int DetectMoveFlags(ChessMove move) {
            int pieceToMoveType = ExtractPieceFromNibble(CurrentBoardState[move.StartPosition.Index]);

            if (pieceToMoveType == PiecePawn) return DetectPawnFlags(move);
            if (pieceToMoveType == PieceKing) return DetectCastleFlag(move);

            return move.Flag;
        }

        private int DetectPawnFlags(ChessMove move) {
            int rankChange = Math.Abs(move.StartPosition.rank - move.EndPosition.rank);
            if (rankChange == 2) return ChessMove.MoveFlag.DoublePush;

            bool isPawnCapture = move.StartPosition.file != move.EndPosition.file;
            if (isPawnCapture) {
                bool isPawnCaptureToEnPassant = CurrentBoardState.EnPassantIndex == move.EndPosition.Index;
                if (isPawnCaptureToEnPassant) return ChessMove.MoveFlag.EnPassant;
            }

            return move.Flag;
        }

        private int DetectCastleFlag(ChessMove move) {
            if (Math.Abs(move.StartPosition.file - move.EndPosition.file) < 2) return move.Flag;

            /// TODO: #21 Free Placement Mode will have to worry about this.
            /// 
            /// If this is a generated move, then these conditions will always be met and trigger
            /// castling.  The ChessGame itself, however, will allow moves that have not been
            /// generated to be submitted and this code under those game conditions will set the
            /// move flag and cause unpredictable behavior (removing pieces, spawning rooks) if
            /// the change in file condition above is met.
            /// 
            int pieceToMove = CurrentBoardState[move.StartPosition.Index];
            ChessTeam pieceToMoveTeam = ExtractTeamFromNibble(pieceToMove);
            bool isQueenside = move.StartPosition.file > move.EndPosition.file;

            if (CurrentBoardState.GetCastleRights(pieceToMoveTeam, isQueenside)) return ChessMove.MoveFlag.Castle;

            return move.Flag;
        }
        #endregion

        #region Game State Updates
        private static bool IsHalfTurnReset(int pieceToMove, int pieceToCapture) =>
            pieceToCapture > 0 || ExtractPieceFromNibble(pieceToMove) == PiecePawn;

        private void UpdateCastleRights(int fromIndex, int toIndex) {
            int fromRank = GetRankFromIndex(fromIndex),
                toRank = GetRankFromIndex(toIndex),

                fileIndex = FileCount - 1,

                teamMultiplier = CurrentBoardState.TeamRankMultiplier,
                otherMultiplier = CurrentBoardState.TeamRankMultiplier ^ 1,

                teamHomeRank = teamMultiplier * fileIndex,
                otherHomeRank = otherMultiplier * fileIndex;

            bool isTeamHomeRank = fromRank == teamHomeRank,
                isOtherHomeRank = toRank == otherHomeRank;

            if (isTeamHomeRank || isOtherHomeRank) {
                int fromFile = GetFileFromIndex(fromIndex),
                    toFile = GetFileFromIndex(toIndex);

                bool isTeamKing = isTeamHomeRank && fromFile == 4;

                if (isTeamKing) CurrentBoardState.RevokeCastleRights(ActiveTeam);
                else {
                    bool isTeamQueenRook = isTeamHomeRank && fromFile == 0,
                        isTeamKingRook = isTeamHomeRank && fromFile == 7,

                        isOtherQueenRook = isOtherHomeRank && toFile == 0,
                        isOtherKingRook = isOtherHomeRank && toFile == 7;

                    if (isTeamKingRook) CurrentBoardState.RevokeCastleRights(ActiveTeam, false);
                    if (isTeamQueenRook) CurrentBoardState.RevokeCastleRights(ActiveTeam, true);
                    if (isOtherKingRook) CurrentBoardState.RevokeCastleRights(OtherTeam, false);
                    if (isOtherQueenRook) CurrentBoardState.RevokeCastleRights(OtherTeam, true);
                }
            }
        }
        #endregion

        #region MoveFlag Processing
        private void ProcessMoveFlags(int toIndex, int flag) {
            int enPassantFileOffset = FileCount * Board.TeamDirectionMultiplier(ActiveTeam);
            int enPassantIndex = toIndex - enPassantFileOffset;

            switch (flag) {
                case ChessMove.MoveFlag.EnPassant:
                    CurrentBoardState[enPassantIndex] = 0;
                    break;
                case ChessMove.MoveFlag.DoublePush:
                    CurrentBoardState.EnPassantIndex = enPassantIndex;
                    break;
                case ChessMove.MoveFlag.Castle:
                    ProcessCastling(toIndex);
                    break;
                case ChessMove.MoveFlag.QueenPromotion:
                case ChessMove.MoveFlag.BishopPromotion:
                case ChessMove.MoveFlag.KnightPromotion:
                case ChessMove.MoveFlag.RookPromotion:
                    CurrentBoardState[toIndex] = GeneratePieceNibble(ActiveTeam, flag);
                    break;
                default: break;
            }
        }

        private void ProcessCastling(int toIndex) {
            const int queensideFileOffset = 2,
                kingsideFileOffset = 6,
                queensideRookOffset = 0,
                kingsideRookOffset = 7;

            int castleRankIndex = GetRankFromIndex(toIndex) * FileCount,

                rookIndex = (
                    (toIndex == castleRankIndex + queensideFileOffset) ?
                        (castleRankIndex + queensideRookOffset)
                        : (castleRankIndex + kingsideRookOffset)
                ),

                rookTargetIndex = (
                    (toIndex == castleRankIndex + queensideFileOffset) ?
                        (castleRankIndex + queensideFileOffset + 1)
                        : (castleRankIndex + kingsideFileOffset - 1)
                );

            CurrentBoardState[rookIndex] = 0;
            CurrentBoardState[rookTargetIndex] =
                GeneratePieceNibble(ActiveTeam, PieceRook);
        }
        #endregion

        public override string ToString() =>
            $"{CurrentGameState}:\n{CurrentBoardState}\n{ExtractCurrentFen(CurrentBoardState)} moves " +
                string.Join(' ', moveList.ConvertAll(m => m.ToString()));
    }
}