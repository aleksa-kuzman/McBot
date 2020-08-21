namespace McBot.Gateway.Payloads
{
    public class GatewayPayload
    {
        /// <summary>
        /// Opcode for payload
        /// </summary>
        public int op { get; set; }

        /// <summary>
        /// Event data
        /// </summary>
        public object d { get; set; }

        public IdentifyRecieveReadyPayload Ready
        {
            get
            {
                if (op == 0)
                    return (IdentifyRecieveReadyPayload)d;
                else
                    return null;
            }
            set { }
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