namespace BulkThumbnailCreator
{
    /// <summary>
    /// Constructor that takes a video url
    /// </summary>
    /// <param name="videoUrl"></param>
    public class Job(string videoUrl, string currentUser)
    {
        public Settings Settings { get; set; }
        public string User { get; set; } = currentUser;
        public List<string> LogEntries { get; set; } = [];
        public States State { get; set; } = States.Initial;
        public Guid JobID { get; set; } = Guid.NewGuid();
        public List<string> FrontLineUpUrls { get; set; } = [];
        public List<string> VarietyUrls { get; set; }
        public string VideoName { get; set; }
        public string VideoUrl { get; set; } = videoUrl;
        public string VideoPath { get; set; }
        public List<PictureData> PictureData { get; set; } = [];
        public List<string> TextToPrint { get; set; } = [];
        public string ClickedImage { get; set; }
    }
}
