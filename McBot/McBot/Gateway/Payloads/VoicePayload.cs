using Newtonsoft.Json;

namespace McBot.Gateway.Payloads
{
    public class VoicePayload
    {
        /// <summary>
        /// Opcode for payload
        /// </summary>
        public VoiceOpCode op { get; set; }

        /// <summary>
        /// Event data
        /// </summary>
        public object d { get; set; }

        /// <summary>
        /// sequence number, used for resuming sessions and heartbeats
        /// </summary>
        public int? s { get; set; }

        /// <summary>
        /// the event name for this payload
        /// </summary>
        public string t { get; set; }

        public VoiceHello VoiceHello
        {
            get
            {
                if (op == VoiceOpCode.Hello)
                {
                    return JsonConvert.DeserializeObject<VoiceHello>(d.ToString());
                }
                else
                    return null;
            }
        }
    }
}