namespace McBot
{
    public class Message
    {
        public Message(string conetnt, bool tts)
        {
            Conetnt = conetnt;
            Tts = tts;
        }

        public string Conetnt { get; set; }
        public bool Tts { get; set; }
    }
}