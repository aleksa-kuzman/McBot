namespace McBot.Gateway.Payloads
{
    public class IdentifyDataPayload
    {
        public string token { get; set; }

        public IdentifyDataPayloadProperties properties { get; set; }
    }
}