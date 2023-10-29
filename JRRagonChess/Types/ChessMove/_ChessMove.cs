using JRRagonGames.Utilities;
using static JRRagonGames.JRRagonChess.Types.ChessMove.MoveFlag;

namespace JRRagonGames.JRRagonChess.Types {
    public readonly partial struct ChessMove {
        public readonly ushort moveData;

        private static class MoveOffset {
            public const int StartIndexOffset = 0x0;
            public const int EndIndexOffset = 0x6;
            public const int FlagIndexOffset = 0xC;
        }

        private static class MoveMask {
            public const int StartIndexMask = 0b111111 << MoveOffset.StartIndexOffset;
            public const int EndIndexMask = 0b111111 << MoveOffset.EndIndexOffset;
            public const int FlagMask = 0b1111 << MoveOffset.FlagIndexOffset;
        }

        public ChessMove(ushort _moveData) { moveData = _moveData; }

        public ChessMove(Position start, Position end, int flag = NoMoveFlag) {
            moveData = (ushort)(0 |
                start.Index << MoveOffset.StartIndexOffset |
                end.Index << MoveOffset.EndIndexOffset |
                flag << MoveOffset.FlagIndexOffset |
            0);
        }

        public ChessMove(string moveText) {
            moveData = (ushort)(0 |
                Position.GetIndex(moveText[..2]) << MoveOffset.StartIndexOffset |
                Position.GetIndex(moveText[2..]) << MoveOffset.EndIndexOffset |
                char.ToLower(moveText[^1]) switch {
                    'q' => QueenPromotion,
                    'b' => BishopPromotion,
                    'r' => RookPromotion,
                    'n' => KnightPromotion,
                    'c' => Castle,
                    'd' => DoublePush,
                    'e' => EnPassant,
                    _ => NoMoveFlag,
                } << MoveOffset.FlagIndexOffset |
            0);
        }

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
    }
}