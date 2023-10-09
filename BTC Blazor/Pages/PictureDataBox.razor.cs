using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace BTC_Blazor.Pages
{
    public partial class PictureDataBox
    {

        public PictureDataBox()
        {

        }

        [Parameter]
        public string ImageURL { get; set; }
        [Parameter]
        public PictureData PicData { get; set; }

        public string FileName { get => PicData.FileName; set => throw new NotImplementedException(); } 
        public string OutPath { get => PicData.OutPath; set => throw new NotImplementedException(); }
        public Box CurrentBox { get => PicData.ParamForTextCreation.CurrentBox; set => throw new NotImplementedException(); }
        public string Font { get => PicData.ParamForTextCreation.Font; set => throw new NotImplementedException(); }
        public OutputType OutPutType { get => PicData.OutPutType; set => throw new NotImplementedException(); }
        public Dictionary<Box, Rectangle> AvailableBoxes { get => PicData.ParamForTextCreation.Boxes; set => throw new NotImplementedException(); }
        public ParamForTextCreation ParamForTextCreation { get => PicData.ParamForTextCreation; set => throw new NotImplementedException(); }
        public List<PictureData> Varieties { get => PicData.Varieties; set => throw new NotImplementedException(); }
        public Box Dankbox { get => PicData.Dankbox; set => throw new NotImplementedException(); }
        public string Meme { get => PicData.Meme; set => throw new NotImplementedException(); }

    }

}


