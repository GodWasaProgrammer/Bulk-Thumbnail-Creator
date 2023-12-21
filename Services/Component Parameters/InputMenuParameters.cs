

using Bulk_Thumbnail_Creator.Enums;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
    public class InputMenuParameters
    {
        public bool DisableMenu { get; set; }
        public bool PassedNull { get; set; }
        public List<string> TextToPrint { get; set; } = new();
        public string VideoUrl { get; set; }

        public List<string> VideoUrls { get; set; } = new();

        public InputMenuParameters()
        {
            DisableMenu = false;
            PassedNull = false;
        }

    }
}
