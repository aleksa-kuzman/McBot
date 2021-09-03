using McBot.Utils.JsonConverter;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McBot.Gateway.Payloads
{
    public class VoicePayload : Payload
    {
        /// <summary>
        /// Opcode for payload
        /// </summary>
        [JsonConverter(typeof(EnumerationClassConverter<VoiceOpCodeEnumeration>))]
        public VoiceOpCodeEnumeration op { get; set; }

        public VoiceHello VoiceHello
        {
            get
            {
                if (op == VoiceOpCodeEnumeration.Hello)
                {
                    return JsonSerializer.Deserialize<VoiceHello>(d.ToString());
                }
                else
                    return null;
            }
        }
    }
}