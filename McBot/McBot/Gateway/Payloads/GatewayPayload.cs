using Newtonsoft.Json;

namespace McBot.Gateway.Payloads
{
    public class GatewayPayload
    {
        /// <summary>
        /// Opcode for payload
        /// </summary>
        public OpCode op { get; set; }

        /// <summary>
        /// Event data
        /// </summary>
        public object d { get; set; }

        public IdentifyRecieveReadyPayload Ready
        {
            get
            {
                if (op == OpCode.Ready)
                    return JsonConvert.DeserializeObject<IdentifyRecieveReadyPayload>(d.ToString());
                else
                    return null;
            }
            set { }
        }

        public GatewayHello GatewayHello
        {
            get
            {
                if (op == OpCode.Hello)
                {
                    return JsonConvert.DeserializeObject<GatewayHello>(d.ToString());
                }
                else
                    return null;
            }
        }

        public MessageCreated MessageCreated
        {
            get
            {
                if (t != null && t == GatewayEvents.MessageCreated.Value)
                {
                    return JsonConvert.DeserializeObject<MessageCreated>(d.ToString());
                }
                else
                    return null;
            }
        }

        public VoiceServerUpdate VoiceServerUpdate
        {
            get
            {
                if (t != null && t == GatewayEvents.VoiceServerUpdate.Value)
                {
                    return JsonConvert.DeserializeObject<VoiceServerUpdate>(d.ToString());
                }
                else
                    return null;
            }
        }

        public VoiceStateUpdate VoiceStateUpdate
        {
            get
            {
                if (t != null && t == GatewayEvents.VoiceStateUpdate.Value)
                {
                    return JsonConvert.DeserializeObject<VoiceStateUpdate>(d.ToString());
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// sequence number, used for resuming sessions and heartbeats
        /// </summary>
        public int? s { get; set; }

        /// <summary>
        /// the event name for this payload
        /// </summary>
        public string t { get; set; }
    }
}