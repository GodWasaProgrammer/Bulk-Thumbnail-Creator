using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator.Component_Parameters
{
    public class VarietyDisplayParameters
    {
        public PictureData ParentPictureProp { get; set; }

        // pass to customize Picture
        public PictureData _passData;

        // pass to customize Picture
        public string passURL;

        public bool CustomPicture = false;

        private List<string> _VarietyUrls = new();
        public List<string> VarietyUrls { get { return _VarietyUrls; } set { _VarietyUrls = value; } }
    }

}
