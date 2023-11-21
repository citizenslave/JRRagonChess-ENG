using JRRagonGames.JRRagonChess.Types;
using System.Text.Json;

namespace JRRagonGames.JRRagonChess.Net.Types {
    public struct PendingGameRequest {
        public ChessTeam team;
        public string uciPosition;
    }

    public struct FindGamePayload {
        public string position;
        public int teamIndex;
        public bool requirePosition;
        public bool requireTeam;
        public string sessionKey;

        public readonly string ToJson() => JsonSerializer.SerializeToElement(
            this,
            new JsonSerializerOptions { IncludeFields = true }
        ).ToString();
    }
}
