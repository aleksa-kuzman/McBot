namespace McBot.HttpApi.Payloads
{
    public class Message
    {
        public Message(string content, bool tts, Embed embed)
        {
            Content = content;
            Tts = tts;
            Embed = embed;
        }

        public string Content { get; set; }
        public bool Tts { get; set; }
        public Embed Embed { get; set; }
    }
}