namespace McBot.Gateway
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