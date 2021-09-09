using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class VoiceIdentify
    {
        public VoiceIdentify(string serverId, string userId, string sessionId, string token, string endpoint)
        {
            ServerId = serverId;
            UserId = userId;
            SessionId = sessionId;
            Token = token;
            Endpoint = "wss://" + endpoint;
        }

        [JsonPropertyName("server_id")]
        public string ServerId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonIgnore]
        public string Endpoint { get; set; }
    }
}