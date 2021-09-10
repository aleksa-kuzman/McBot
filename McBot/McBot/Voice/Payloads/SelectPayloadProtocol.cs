using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class SelectPayloadProtocol
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("data")]
        public SelectPayloadProtocolData Data { get; set; }
    }
}