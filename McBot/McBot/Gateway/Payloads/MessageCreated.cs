using System.Text.Json.Serialization;

namespace McBot.Gateway.Payloads
{
    public class MessageCreated
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}