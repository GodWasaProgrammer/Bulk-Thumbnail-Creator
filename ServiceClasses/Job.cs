namespace BulkThumbnailCreator
{
    /// <summary>
    /// Constructor that takes a video url
    /// <param name="videoUrl"></param>
    public class Job(string videoUrl, string currentUser)
    {
        public delegate void StateChangedEventHandler(object sender, EventArgs e);
        public event StateChangedEventHandler StateChanged;

        public Settings Settings { get; set; }
        public string User { get; set; } = currentUser;
        public List<string> LogEntries { get; set; } = [];

        protected virtual void OnStateChanged(EventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        private States _state;
        public States State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged(EventArgs.Empty); // Raise the event
                }
            }
        }
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
