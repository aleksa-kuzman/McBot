using McBot.Gateway.Payloads;
using Newtonsoft.Json;
using System;

namespace McBot.Core
{
    public class Payload
    {
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
            return JsonConvert.DeserializeObject<T>(d.ToString());
        }
    }
}