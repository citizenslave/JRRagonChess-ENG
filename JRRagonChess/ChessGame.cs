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
        public int this[int boardTileIndex] => boardState[boardTileIndex];



        public GameState CurrentGameState { get; private set; }



        public ChessTeam ActiveTeam => boardState.ActiveChessTeam;
        public ChessTeam OtherTeam => boardState.OtherChessTeam;



        public string FenCode => ExtractCurrentFen(boardState);
        #endregion



        #region Constructors and Factories
        public ChessGame() : this(ParseFen(startpos)) { }
        public ChessGame(string fenCode) : this(ParseFen(fenCode)) { }
        public ChessGame(Board boardState, bool isSimulated = false) {
            this.boardState = boardState;
            if (!isSimulated) UpdateGameState();
        }

        public ChessGame GetSimulation() => new ChessGame(boardState.Copy(), true);
        private readonly Board boardState;
        #endregion



        #region Move List
        public IReadOnlyList<ChessMove> MoveList => moveList.AsReadOnly();
        private readonly List<ChessMove> moveList = new List<ChessMove>();
        #endregion



        #region MoveGenerator Interface
        public List<ChessMove> GetAllLegalMoves() => new MoveGenerator(this).GenerateAllMoves();
        public List<ChessMove> GetLegalMovesFrom(Position position) => new List<ChessMove>();
        public List<ChessMove> GetPseudoLegalMovesFrom(Position position) => boardState.GetPseudoLegalMovesFrom(position);
        #endregion



        #region Public Interface
        public int CapturedPieceIndex(ChessMove move) => move.Flag switch {





            ChessMove.MoveFlag.EnPassant => move.EndPosition.Index - (Board.TeamDirectionMultiplier(OtherTeam) * FileCount),




            _ => move.EndPosition.Index
        };



        public void ExecuteMove(ChessMove move, bool simulated = false) {
            if (!boardState.IsMoveValid(move)) return;
            


            int fromIndex = move.StartPosition.Index;
            int toIndex = move.EndPosition.Index;
            int pieceToMove = boardState[fromIndex];
            int flag = DetectMoveFlags(move);



            bool isHalfTurnReset = false ||
                boardState[toIndex] > PieceNone ||
                ExtractPieceFromNibble(pieceToMove) == PiecePawn ||
            false;
            if (isHalfTurnReset) boardState.HalfCount = 0;
            else boardState.HalfCount++;



            boardState[fromIndex] = 0;
            boardState[toIndex] = pieceToMove;
            moveList.Add(move);



            boardState.EnPassant = 0;
            UpdateCastleRights(fromIndex, toIndex);
            ProcessMoveFlags(toIndex, flag);



            if (boardState.ActiveChessTeam == ChessTeam.BlackTeam) boardState.TurnCount++;            
            boardState.ActiveChessTeam = boardState.OtherChessTeam;
            if (!simulated) UpdateGameState();
        }



        public bool IsInCheck(bool flip = true) {
            Board flipTurnBoard = boardState.Copy();
            if (flip) flipTurnBoard.ActiveChessTeam = flipTurnBoard.OtherChessTeam;
            MoveGenerator endGameSimulator = new MoveGenerator(new ChessGame(flipTurnBoard, true));
            return endGameSimulator.IsPositionThreatened(flipTurnBoard.OtherKingPosition);
        }
        #endregion



        #region Move Flags
        #region MoveFlag Detection
        private int DetectMoveFlags(ChessMove move) {
            int pieceToMoveType = ExtractPieceFromNibble(boardState[move.StartPosition.Index]);

            if (pieceToMoveType == PiecePawn) return DetectPawnFlags(move);
            if (pieceToMoveType == PieceKing) return DetectCastleFlag(move);

            return move.Flag;
        }

        private int DetectPawnFlags(ChessMove move) {
            int rankChange = Math.Abs(move.StartPosition.rank - move.EndPosition.rank);
            if (rankChange == 2) return ChessMove.MoveFlag.DoublePush;

            bool isPawnCapture = move.StartPosition.file != move.EndPosition.file;
            if (isPawnCapture) {
                bool isPawnCaptureToEnPassant = boardState.EnPassantIndex == move.EndPosition.Index;
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
            int pieceToMove = boardState[move.StartPosition.Index];
            ChessTeam pieceToMoveTeam = ExtractTeamFromNibble(pieceToMove);
            bool isQueenside = move.StartPosition.file > move.EndPosition.file;



            if (boardState.GetCastleRights(pieceToMoveTeam, isQueenside)) return ChessMove.MoveFlag.Castle;



            return move.Flag;
        }
        #endregion

        #region MoveFlag Processing
        private void ProcessMoveFlags(int toIndex, int flag) {
            int enPassantFileOffset = FileCount * Board.TeamDirectionMultiplier(ActiveTeam);
            int enPassantIndex = toIndex - enPassantFileOffset;

            switch (flag) {
                case ChessMove.MoveFlag.EnPassant:
                    boardState[enPassantIndex] = 0;
                    break;
                case ChessMove.MoveFlag.DoublePush:
                    boardState.EnPassantIndex = enPassantIndex;
                    break;
                case ChessMove.MoveFlag.Castle:
                    ProcessCastling(toIndex);
                    break;
                case ChessMove.MoveFlag.QueenPromotion:
                case ChessMove.MoveFlag.BishopPromotion:
                case ChessMove.MoveFlag.KnightPromotion:
                case ChessMove.MoveFlag.RookPromotion:
                    boardState[toIndex] = GeneratePieceNibble(ActiveTeam, flag);
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



            boardState[rookIndex] = 0;
            boardState[rookTargetIndex] =
                GeneratePieceNibble(ActiveTeam, PieceRook);
        }
        #endregion
        #endregion



        #region Game State Updates
        private void UpdateCastleRights(int fromIndex, int toIndex) {
            int fromRank = GetRankFromIndex(fromIndex),
                toRank = GetRankFromIndex(toIndex),

                homeRankIndex = FileCount - 1,

                teamMultiplier = boardState.ActiveTeamIndex,
                otherMultiplier = boardState.OtherTeamIndex,

                teamHomeRank = teamMultiplier * homeRankIndex,
                otherHomeRank = otherMultiplier * homeRankIndex;

            bool isTeamHomeRank = fromRank == teamHomeRank,
                isOtherHomeRank = toRank == otherHomeRank;

            if (isTeamHomeRank || isOtherHomeRank) {
                int fromFile = GetFileFromIndex(fromIndex),
                    toFile = GetFileFromIndex(toIndex);

                bool isTeamKing = isTeamHomeRank && fromFile == 4;

                if (isTeamKing) boardState.RevokeCastleRights(ActiveTeam);
                else {
                    bool isTeamQueenRook = isTeamHomeRank && fromFile == 0,
                        isTeamKingRook = isTeamHomeRank && fromFile == 7,

                        isOtherQueenRook = isOtherHomeRank && toFile == 0,
                        isOtherKingRook = isOtherHomeRank && toFile == 7;

                    if (isTeamKingRook) boardState.RevokeCastleRights(ActiveTeam, false);
                    if (isTeamQueenRook) boardState.RevokeCastleRights(ActiveTeam, true);
                    if (isOtherKingRook) boardState.RevokeCastleRights(OtherTeam, false);
                    if (isOtherQueenRook) boardState.RevokeCastleRights(OtherTeam, true);
                }
            }
        }

        private void UpdateGameState() {
            MoveGenerator endGameSimulator = new MoveGenerator(this);
            if (endGameSimulator.GenerateAllMoves().Count == 0)
                CurrentGameState = IsInCheck() ? GameState.Checkmate : GameState.Stalemate;






            /// TODO: Allow a draw to be claimed unilaterally at 100
            /// 
            else if (boardState.HalfCount > 150) CurrentGameState = GameState.Progress;
            //else if (CurrentBoardState.HalfTurn < 100 && CurrentGameState == GameState.Progress) CurrentGameState = GameState.Running;










            else {

                /// TODO: Implement more of this.  Two kings is the obvious material draw, but the minor piece cases
                /// need to be handled as well and this is not doing that.  Extract to Board?
                int pieceCounter = 0;
                for (int tileIndex = 0; tileIndex < TileCount; tileIndex++) {
                    if (boardState[tileIndex] != PieceNone) pieceCounter++;
                    /// TODO: Minor Piece counters.  Extract to Board?
                }
                if (pieceCounter == 2) CurrentGameState = GameState.Material;






                /// TODO: 3-5 Fold Repetition Draw Condition
                /// 

            }


        }
        #endregion



        public override string ToString() =>
            $"{CurrentGameState}:\n{boardState}\n{ExtractCurrentFen(boardState)} moves " +
                string.Join(' ', moveList.ConvertAll(m => m.ToString()));
    }
}
