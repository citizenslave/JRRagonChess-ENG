using JRRagonGames.JRRagonChess.Types;



namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static partial class FenUtility {
        private static class BoardSerializer {
            public static byte[] SerializeBoard(BoardState.Board board) {
                int gameData = board.GameData;
                byte[] serializedBoard = new byte[BoardConstants.TileCount / 2 + (sizeof(int) - 1)];



                int byteIndex = 0, nibbleSize = 4, byteOffset = nibbleSize, pieceIndex = 0;
                while (pieceIndex < BoardConstants.TileCount) {
                    serializedBoard[byteIndex] |= (byte)(board[pieceIndex++] << byteOffset);



                    byteOffset ^= nibbleSize;
                    if (byteOffset == nibbleSize) byteIndex++;
                }



                serializedBoard[byteIndex++] = (byte)(gameData >> 16);
                serializedBoard[byteIndex++] = (byte)(gameData >> 8);
                serializedBoard[byteIndex++] = (byte)(gameData >> 0);



                return serializedBoard;
            }

            public static BoardState.Board DeserializeBoard(byte[] serializedBoard) {
                int[] pieces = new int[BoardState.Board.Constants.TileCount];
                int gameData;



                for (int pieceIndex = 0; pieceIndex < BoardState.Board.Constants.TileCount; pieceIndex++)
                    pieces[pieceIndex] = serializedBoard[pieceIndex / 2] >> (pieceIndex % 2 ^ 1) * 4 & 0xf;

                gameData = serializedBoard[32] << 16 | serializedBoard[33] << 8 | serializedBoard[34];



                return new BoardState.Board(pieces, gameData);
            }
        }
    }
}
