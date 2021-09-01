using McBot.Utils.JsonConverter;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McBot.Gateway.Payloads
{
    public class GatewayPayload : Payload
    {
        /// <summary>
        /// Opcode for payload
        /// </summary>
        [JsonConverter(typeof(EnumerationClassConverter<OpCodeEnumeration>))]
        public OpCodeEnumeration op { get; set; }

        public IdentifyRecieveReadyPayload Ready
        {
            get
            {
                if (op == OpCodeEnumeration.Ready)
                    return JsonSerializer.Deserialize<IdentifyRecieveReadyPayload>(d.ToString());
                else
                    return null;
            }
            set { }
        }

        public GatewayHello GatewayHello
        {
            get
            {
                if (op == OpCodeEnumeration.Hello)
                {
                    return JsonSerializer.Deserialize<GatewayHello>(d.ToString());
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
                    return JsonSerializer.Deserialize<MessageCreated>(d.ToString());
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
                    return JsonSerializer.Deserialize<VoiceServerUpdate>(d.ToString());
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
                    return JsonSerializer.Deserialize<VoiceStateUpdate>(d.ToString());
                }
                else
                    return null;
            }
        }

        public Type GetPayloadType()
        {
            foreach (var item in GatewayEvents.GetGatewayEvents())
            {
                if (item.Value == t)
                {
                    return item.Type;
                }
            }
            throw new Exception();
        }

        public T GetPayload<T>()
        {
            return JsonSerializer.Deserialize<T>(d.ToString());
        }
    }
}