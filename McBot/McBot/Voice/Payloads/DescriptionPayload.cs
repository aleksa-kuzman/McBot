using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class DescriptionPayload
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("secret_key")]
        public byte[] SecretKey { get; set; }
    }
}