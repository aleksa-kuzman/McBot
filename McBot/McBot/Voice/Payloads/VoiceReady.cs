using McBot.Utils.JsonConverter;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace McBot.Voice.Payloads
{
    public class VoiceReady
    {
        [JsonPropertyName("ssrc")]
        public int Ssrc { get; set; }

        [JsonPropertyName("ip")]
        [JsonConverter(typeof(IPAddresConverter))]
        public IPAddress Ip { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("modes")]
        public IList<string> Modes { get; set; }
    }
}