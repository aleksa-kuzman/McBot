namespace McBot.Gateway.Payloads
{
    public class IdentifyRecieveReadyPayload
    {
        /// <summary>
        /// Gateway version
        /// </summary>
        public int v { get; set; }

        /// <summary>
        /// Need to implement user object
        /// </summary>
        public object user { get; set; }

        /// <summary>
        /// Don't know what i will use it for yet
        /// </summary>
        public object[] private_channels { get; set; }

        /// <summary>
        /// Don't know what i will use it for yet
        /// </summary>
        public object guilds { get; set; }

        /// <summary>
        /// This is id of current session used to later resume broken conneciton
        /// </summary>
        public string session_id { get; set; }

        /// <summary>
        /// These are used for large bots but i still don't use them
        /// </summary>
        public int[] shard { get; set; }
    }
}