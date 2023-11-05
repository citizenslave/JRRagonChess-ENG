using System;

namespace JRRagonGames.JRRagonChess.ChessUtils {
    public static class UciUtility {
        public static ChessGame UciPosition(string position) {
            string[] tokens = position.Trim().Split(' ');

            if (tokens.Length < 2 || tokens[0] != "position") return new ChessGame();

            ChessGame game;
            if (tokens[1] == "startpos" || tokens[1] != "fen" || tokens.Length < 8) game = new ChessGame();
            else game = new ChessGame(string.Join(' ', tokens[2..8]));

            int movesIndex = Array.IndexOf(tokens, "moves") + 1;
            if (movesIndex == 0) return game;

            while (movesIndex < tokens.Length) game.ExecuteMove(new Types.ChessMove(tokens[movesIndex++]));

            return game;
        }
    }
}
