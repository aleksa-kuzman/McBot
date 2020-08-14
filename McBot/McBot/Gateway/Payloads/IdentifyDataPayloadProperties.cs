namespace McBot.Gateway.Payloads
{
    public class IdentifyDataPayloadProperties
    {
        public IdentifyDataPayloadProperties(string os, string browser, string device)
        {
            this.os = os;
            this.browser = browser;
            this.device = device;
        }

        /// <summary>
        /// Operating system
        /// </summary>
        public string os { get; set; }

        /// <summary>
        /// browser on which client runs
        /// </summary>
        public string browser { get; set; }

        /// <summary>
        /// device on which client runs
        /// </summary>
        public string device { get; set; }
    }
}