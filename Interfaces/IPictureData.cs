using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator.InterFaces
{
    public interface IPictureData
    {
        PictureData PicData { get; }
        string FileName { get; }
        string OutPath { get; }
        public ParamForTextCreation ParamForTextCreation { get; }
        public List<PictureData> Varieties { get; }
        public OutputType OutPutType { get; }
        public Box Dankbox { get; }
        public string Meme { get; }
    }

}