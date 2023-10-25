using JRRagonGames.JRRagonChess.Utilities;
using JRRagonGames.JRRagonChess.BoardState;
using static JRRagonGames.JRRagonChess.ChessMove.MoveFlag;

namespace JRRagonGames.JRRagonChess {
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

        public ChessMove(ushort _moveData) { this.moveData = _moveData; }

        public ChessMove(Position start, Position end, int flag = NoMoveFlag) {
            moveData = (ushort)(0 |
                start.Index << MoveOffset.StartIndexOffset |
                end.Index << MoveOffset.EndIndexOffset |
                flag << MoveOffset.FlagIndexOffset |
            0);
        }

        public ChessMove(string moveText) {
            moveData = (ushort)(0 |
                (Position.GetIndex(moveText[..2]) << MoveOffset.StartIndexOffset) |
                (Position.GetIndex(moveText[2..]) << MoveOffset.EndIndexOffset) |
                (char.ToLower(moveText[^1]) switch {
                    'q' => QueenPromotion,
                    'b' => BishopPromotion,
                    'r' => RookPromotion,
                    'n' => KnightPromotion,
                    'c' => Castle,
                    'd' => DoublePush,
                    'e' => EnPassant,
                    _ => NoMoveFlag,
                } << MoveOffset.FlagIndexOffset) |
            0);
        }

        public Position StartPosition {
            get => Position.GetPositionFromIndex(
                BitUtilities.GetBits(
                    moveData,
                    MoveOffset.StartIndexOffset,
                    MoveMask.StartIndexMask
                )
            );
        }

        public Position EndPosition {
            get => Position.GetPositionFromIndex(
                BitUtilities.GetBits(
                    moveData,
                    MoveOffset.EndIndexOffset,
                    MoveMask.EndIndexMask
                )
            );
        }

        public int Flag {
            get => BitUtilities.GetBits(
                moveData,
                MoveOffset.FlagIndexOffset,
                MoveMask.FlagMask
            );
        }

        public override string ToString() {
            return ToString(false);
        }

        public string ToString(bool useFullLAN) => Flag switch {
            MoveFlag.Castle => useFullLAN ? (EndPosition.file > StartPosition.file ? "O-O" : "O-O-O") : GetMoveText(),
            MoveFlag.EnPassant => GetMoveText() + (useFullLAN ? "ep" : ""),
            MoveFlag.KnightPromotion => GetMoveText() + "N",
            MoveFlag.RookPromotion => GetMoveText() + "R",
            MoveFlag.BishopPromotion => GetMoveText() + "B",
            MoveFlag.QueenPromotion => GetMoveText() + "Q",
            _ => GetMoveText(),
        };

        private string GetMoveText() => StartPosition.ToString() + EndPosition.ToString();
    }
}