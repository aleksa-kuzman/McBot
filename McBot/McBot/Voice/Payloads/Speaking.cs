using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class Speaking
    {
        [JsonPropertyName("speaking")]
        public int SpeakingFlag { get; set; }

        [JsonPropertyName("delay")]
        public int Delay { get; set; }

        [JsonPropertyName("ssrc")]
        public int Ssrc { get; set; }
    }
}