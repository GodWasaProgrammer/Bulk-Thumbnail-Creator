using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.InterFaces;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace BTC_Blazor.Pages
{
    public partial class PictureDataBox : IPictureData
    {
        [Parameter]
        public PictureData PicData { get; set; }
        public string FileName { get; set; }
        public string OutPath { get; set; }
        public Box CurrentBox { get; set; }
        public string Font { get; set; }
        public OutputType OutPutType { get => PicData.OutPutType; set => throw new NotImplementedException(); }
        public Dictionary<Box, Rectangle> AvailableBoxes { get; set; }
        public ParamForTextCreation ParamForTextCreation { get => PicData.ParamForTextCreation; set => throw new NotImplementedException(); }
        public List<PictureData> Varieties { get => PicData.Varieties; set => throw new NotImplementedException(); }
        public Box Dankbox { get => PicData.Dankbox; set => throw new NotImplementedException(); }
        public string Meme { get => PicData.Meme; set => throw new NotImplementedException(); }

    }

}


