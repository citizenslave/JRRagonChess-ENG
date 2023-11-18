using JRRagonGames.Utilities;

using static JRRagonGames.JRRagonChess.Types.ChessMove.MoveFlag;



namespace JRRagonGames.JRRagonChess.Types {
    public readonly partial struct ChessMove {
        public readonly ushort moveData;

        #region Constructors
        public ChessMove(ushort _moveData) { moveData = _moveData; }
        public ChessMove(Position start, Position end, int flag = NoMoveFlag) : this(
            (ushort)(0 |
                start.Index << MoveOffset.StartIndexOffset |
                end.Index << MoveOffset.EndIndexOffset |
                flag << MoveOffset.FlagIndexOffset |
            0)
        ) { }
        public ChessMove(string moveText) : this(
            Position.GetPositionFromName(moveText[..2]),
            Position.GetPositionFromName(moveText[2..4]),
            char.ToLower(moveText[^1]) switch {
                'q' => QueenPromotion,
                'b' => BishopPromotion,
                'r' => RookPromotion,
                'n' => KnightPromotion,
                'c' => Castle,
                'd' => DoublePush,
                'e' => EnPassant,
                _ => NoMoveFlag,
            }
        ) { }
        #endregion



        #region Accessors
        public Position StartPosition => Position.GetPositionFromIndex(
            BitUtilities.GetBits(
                moveData,
                MoveOffset.StartIndexOffset,
                MoveMask.StartIndexMask
            )
        );

        public Position EndPosition => Position.GetPositionFromIndex(
            BitUtilities.GetBits(
                moveData,
                MoveOffset.EndIndexOffset,
                MoveMask.EndIndexMask
            )
        );

        public int Flag => BitUtilities.GetBits(
            moveData,
            MoveOffset.FlagIndexOffset,
            MoveMask.FlagMask
        );
        #endregion



        #region ToString & Helpers
        public override string ToString() => ToString(false);

        public string ToString(bool useFullLAN) => Flag switch {
            Castle => useFullLAN ? EndPosition.file > StartPosition.file ? "O-O" : "O-O-O" : GetMoveText(),
            EnPassant => GetMoveText() + (useFullLAN ? "ep" : ""),
            KnightPromotion => GetMoveText() + "N",
            RookPromotion => GetMoveText() + "R",
            BishopPromotion => GetMoveText() + "B",
            QueenPromotion => GetMoveText() + "Q",
            _ => GetMoveText(),
        };

        private string GetMoveText() => StartPosition.ToString() + EndPosition.ToString();
        #endregion
    }
}
