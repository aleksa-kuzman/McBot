using Newtonsoft.Json;

namespace McBot.Gateway.Payloads
{
    public class VoiceIdentify
    {
        public VoiceIdentify(string serverId, string userId, string sessionId, string token)
        {
            ServerId = serverId;
            UserId = userId;
            SessionId = sessionId;
            Token = token;
        }

        [JsonProperty("server_id")]
        public string ServerId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}