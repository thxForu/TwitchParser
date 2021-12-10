namespace TwitchParser
{
    public class Clips
    {
        public Curator Curator { get; set; }
        public string Url { get; set; }
        public Broadcaster Broadcaster { get; set; }
        public string Game { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public string Title { get; set; }
    }
}   