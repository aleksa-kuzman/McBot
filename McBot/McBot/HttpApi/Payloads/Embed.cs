namespace McBot.HttpApi.Payloads
{
    public class Embed
    {
        public Embed(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}