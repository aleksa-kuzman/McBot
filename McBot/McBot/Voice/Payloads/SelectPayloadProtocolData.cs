using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class SelectPayloadProtocolData
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public ushort Port { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }
    }
}