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
    }
}