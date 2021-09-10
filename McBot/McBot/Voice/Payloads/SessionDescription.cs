using McBot.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class SessionDescription
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonConverter(typeof(ByteArrayConverter))]
        [JsonPropertyName("secret_key")]
        public byte[] SecretKey { get; set; }
    }
}