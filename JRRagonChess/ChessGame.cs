using System;
using System.Collections.Generic;

using JRRagonGames.JRRagonChess.Types;
using JRRagonGames.JRRagonChess.BoardState;

using static JRRagonGames.JRRagonChess.Types.PieceUtil;
using static JRRagonGames.JRRagonChess.Types.Position;
using static JRRagonGames.JRRagonChess.Types.BoardConstants;

using static JRRagonGames.JRRagonChess.ChessUtils.FenUtility;



namespace JRRagonGames.JRRagonChess {
    public class ChessGame {
        #region Public Accessors
        public int this[int boardTileIndex] => CurrentBoardState[boardTileIndex];



        public GameState CurrentGameState { get; private set; }



        public ChessTeam ActiveTeam => CurrentBoardState.ActiveChessTeam;
        public ChessTeam OtherTeam => CurrentBoardState.OtherChessTeam;



        public string FenCode => ExtractCurrentFen(CurrentBoardState);
        #endregion



        #region Constructors and Factories
        public ChessGame() : this(ParseFen(startpos)) { }
        public ChessGame(string fenCode) : this(ParseFen(fenCode)) { }
        public ChessGame(Board boardState, bool isSimulated = false) {
            CurrentBoardState = boardState;
            if (!isSimulated) UpdateGameState();
        }

        public ChessGame GetSimulation() => new ChessGame(CurrentBoardState.Copy(), true);
        #endregion



        /// TODO: Make this private.
        ///
        public Board CurrentBoardState { get; private set; }

        #region Move List
        public IReadOnlyList<ChessMove> MoveList => moveList.AsReadOnly();
        private readonly List<ChessMove> moveList = new List<ChessMove>();
        #endregion



        #region MoveGenerator Interface
        public List<ChessMove> GetAllLegalMoves() => new MoveGenerator(this).GenerateAllMoves();
        public List<ChessMove> GetLegalMovesFrom(Position position) => new List<ChessMove>();
        public List<ChessMove> GetPseudoLegalMovesFrom(Position position) => CurrentBoardState.GetPseudoLegalMovesFrom(position);
        #endregion



        public int CapturedPieceIndex(ChessMove move) => move.Flag switch {





            ChessMove.MoveFlag.EnPassant => move.EndPosition.Index - (Board.TeamDirectionMultiplier(OtherTeam) * FileCount),




            _ => move.EndPosition.Index
        };



        public void ExecuteMove(ChessMove move, bool simulated = false) {
            if (!CurrentBoardState.IsMoveValid(move)) return;
            


            int fromIndex = move.StartPosition.Index;
            int toIndex = move.EndPosition.Index;
            int pieceToMove = CurrentBoardState[fromIndex];
            int flag = DetectMoveFlags(move);



            bool isHalfTurnReset = false ||
                CurrentBoardState[toIndex] > PieceNone ||
                ExtractPieceFromNibble(pieceToMove) == PiecePawn ||
            false;
            if (isHalfTurnReset) CurrentBoardState.HalfCount = 0;
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



        #region Move Flags
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
        #endregion



        #region Game State Updates
        public bool IsInCheck() {
            Board flipTurnBoard = CurrentBoardState.Copy();
            flipTurnBoard.ActiveChessTeam = flipTurnBoard.OtherChessTeam;
            MoveGenerator endGameSimulator = new MoveGenerator(new ChessGame(flipTurnBoard));
            return endGameSimulator.IsInCheck();
        }



        private void UpdateCastleRights(int fromIndex, int toIndex) {
            int fromRank = GetRankFromIndex(fromIndex),
                toRank = GetRankFromIndex(toIndex),

                homeRankIndex = FileCount - 1,

                teamMultiplier = CurrentBoardState.ActiveTeamIndex,
                otherMultiplier = CurrentBoardState.OtherTeamIndex,

                teamHomeRank = teamMultiplier * homeRankIndex,
                otherHomeRank = otherMultiplier * homeRankIndex;

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

        private void UpdateGameState() {
            MoveGenerator endGameSimulator = new MoveGenerator(this);
            if (endGameSimulator.GenerateAllMoves().Count == 0)
                CurrentGameState = IsInCheck() ? GameState.Checkmate : GameState.Stalemate;






            /// TODO: Implement this properly.  The flag will immediately end the game and the straight counter
            /// overflows at 127, 23 plys before the progress draw is arbitrated.  The HT counter needs at least
            /// one more bit (which could be used to represent the flag in the game data)
            else if (CurrentBoardState.HalfCount > 100) CurrentGameState = GameState.Progress;
            //else if (CurrentBoardState.HalfTurn < 100 && CurrentGameState == GameState.Progress) CurrentGameState = GameState.Running;










            else {

                /// TODO: Implement more of this.  Two kings is the obvious material draw, but the minor piece cases
                /// need to be handled as well and this is not doing that.  Extract to Board?
                int pieceCounter = 0;
                for (int tileIndex = 0; tileIndex < TileCount; tileIndex++) {
                    if (CurrentBoardState[tileIndex] != PieceNone) pieceCounter++;
                    /// TODO: Minor Piece counters.  Extract to Board?
                }
                if (pieceCounter == 2) CurrentGameState = GameState.Material;






                /// TODO: 3-5 Fold Repetition Draw Condition
                /// 

            }


        }
        #endregion



        public override string ToString() =>
            $"{CurrentGameState}:\n{CurrentBoardState}\n{ExtractCurrentFen(CurrentBoardState)} moves " +
                string.Join(' ', moveList.ConvertAll(m => m.ToString()));
    }
}
