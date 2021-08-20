namespace McBot.Gateway.Payloads
{
    internal class GatewayEvents
    {
        public string Value { get; set; }

        public GatewayEvents(string value)
        {
            Value = value;
        }

        public static GatewayEvents MessageCreated { get { return new GatewayEvents("MESSAGE_CREATE"); } }
        public static GatewayEvents VoiceServerUpdate { get { return new GatewayEvents("VOICE_SERVER_UPDATE"); } }
        public static GatewayEvents VoiceStateUpdate { get { return new GatewayEvents("VOICE_STATE_UPDATE"); } }
    }
}