using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator.Services.Component_Parameters
{
    public class PictureDataBoxParameters
    {
        public string imageUrl { get; set; }
        public PictureData PictureData { get; set; } = new PictureData();
    }
}
