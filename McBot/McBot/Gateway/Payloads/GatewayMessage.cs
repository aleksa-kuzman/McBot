using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace McBot.Gateway.Payloads
{
    public class GatewayMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("guild_id?")]
        public string GuildId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonPropertyName("edited_timestamp")]
        public DateTime? EditedTimeStamp { get; set; }

        [JsonPropertyName("tts")]
        public bool Tts { get; set; }

        [JsonPropertyName("mention_everyone")]
        public bool MentionEveryone { get; set; }

        [JsonPropertyName("mentions")]
        public IEnumerable<User> Mentions { get; set; }
    }
}