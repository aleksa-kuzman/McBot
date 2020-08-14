namespace McBot.Gateway.Payloads
{
    public class IdentifyDataPayload
    {
        public string Token { get; set; }

        public IdentifyDataPayloadProperties Properties { get; set; }
    }
}