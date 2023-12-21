using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator.Component_Parameters
{
    public class ImageDisplayParameters
    {
        private bool imageDisplayClicked = false;
        public bool ImageDisplayClicked
        {
            get { return imageDisplayClicked; }
            set { imageDisplayClicked = value; }
        }

        public PictureData PictureData { get; set; } = new PictureData();

        public List<string> imageUrls = new List<string>();

        public List<string> VarietyUrls = new List<string>();

        VarietyDisplayParameters VarietyDisplayParameters = new VarietyDisplayParameters();
    }

}
