using System.Text.Json.Serialization;

namespace McBot.Gateway.Payloads
{
    public class VoiceServerUpdate
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }
    }
}