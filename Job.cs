using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
    public class Job
    {
        States state { get; set; }
        Guid JobID { get; set; }
        string Videoname { get; set; }
        string VideoUrl { get; set; }
        string VideoPath { get; set; }

        List<PictureData> pictureDatas { get; set; }
        List<string> TextToPrint { get; set; }
    }

}
