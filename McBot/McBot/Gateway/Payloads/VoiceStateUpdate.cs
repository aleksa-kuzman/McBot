using System.Text.Json.Serialization;

namespace McBot.Gateway.Payloads
{
    public class VoiceStateUpdate
    {
        public VoiceStateUpdate()
        {
        }

        public VoiceStateUpdate(string guildId, string channelId, bool selfMute = false, bool selfDeaf = false)
        {
            GuildId = guildId;
            ChannelId = channelId;
            SelfMute = selfMute;
            SelfDeaf = selfDeaf;
        }

        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("session_id")]
        public string SesssionId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("server_id")]
        public string ServerId { get; set; }

        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; set; }

        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; set; }
    }
}