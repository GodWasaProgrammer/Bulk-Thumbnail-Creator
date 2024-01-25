using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
    public class Job
    {
        public Settings Settings { get; set; }

        /// <summary>
        /// not implemented yet
        /// </summary>
        private string _User;
        public string User { get { return _User; } set { _User = value; } }

        /// <summary>
        /// The list of log entries for this job to be able to save state between refresh/closings etc
        /// </summary>
        private List<string> _LogEntries = new List<string>();
        public List<string> LogEntries { get { return _LogEntries; } set { _LogEntries = value; } }

        private States _State;
        public States State { get { return _State; } set { _State = value; } }

        /// <summary>
        /// Unique Job ID
        /// </summary>
        private Guid _JobID;
        public Guid JobID { get { return _JobID; } set { _JobID = value; } }

        public List<string> VideoUrls { get; set; } = new List<string>();

        /// <summary>
        /// List of Varieties created on the VarietyDisplay page
        /// </summary>
        /// 
        private List<string> _VarietyUrls;
        public List<string> VarietyUrls { get { return _VarietyUrls; } set { _VarietyUrls = value; } }

        /// <summary>
        /// The videos name on youtube and on disk
        /// </summary>
        private string _VideoName;
        public string VideoName { get { return _VideoName; } set { _VideoName = value; } }

        /// <summary>
        /// The link for the downloaded video
        /// </summary>
        private string _VideoUrl;
        public string VideoUrl { get { return _VideoUrl; } set { _VideoUrl = value; } }

        /// <summary>
        /// Path to the downloaded video
        /// </summary>
        private string _VideoPath;
        public string VideoPath { get { return _VideoPath; } set { _VideoPath = value; } }

        /// <summary>
        /// List of our picture data objects which contains all the information we need to create the pictures
        /// </summary>
        private List<PictureData> _pictureDatas;
        public List<PictureData> PictureDatas { get { return _pictureDatas; } set { _pictureDatas = value; } }

        /// <summary>
        /// The text to print on the picture
        /// </summary>
        private List<string> _TextToPrint;
        public List<string> TextToPrint { get { return _TextToPrint; } set { _TextToPrint = value; } }

        /// <summary>
        /// This is the image that has been clicked on and pushed to CustomPicture
        /// </summary>
        private string _ClickedImage;
        public string ClickedImage { get { return _ClickedImage; } set { _ClickedImage = value; } }

        /// <summary>
        /// Constructor that takes a video url
        /// </summary>
        /// <param name="VideoUrl"></param>
        public Job(string VideoUrl)
        {
            State = States.Initial;
            JobID = Guid.NewGuid();
            this.VideoUrl = VideoUrl;
            PictureDatas = new List<PictureData>();
            TextToPrint = new List<string>();
        }

    }

}