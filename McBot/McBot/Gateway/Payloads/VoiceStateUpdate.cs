using Newtonsoft.Json;

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

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }

        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }
    }
}