using Newtonsoft.Json;

namespace McBot.Gateway.Payloads
{
    public class VoiceServerUpdate
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
    }
}