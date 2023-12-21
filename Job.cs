using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
    public class Job
    {
        private States _State;
        public States State { get { return _State; } set { _State = value; } }

        private Guid _JobID;
        public Guid JobID { get { return _JobID; } set { _JobID = value; } }


        private string _VideoName;
        public string VideoName { get { return _VideoName; } set { _VideoName = value; } }


        private string _VideoUrl;
        public string VideoUrl { get { return _VideoUrl; } set { _VideoUrl = value; } }


        private string _VideoPath;
        public string VideoPath { get { return _VideoPath; } set { _VideoPath = value; } }


        private List<PictureData> _pictureDatas;
        public List<PictureData> PictureDatas { get { return _pictureDatas; } set { _pictureDatas = value; } }


        private List<string> _TextToPrint;
        public List<string> TextToPrint { get { return _TextToPrint; } set { _TextToPrint = value; } }


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
