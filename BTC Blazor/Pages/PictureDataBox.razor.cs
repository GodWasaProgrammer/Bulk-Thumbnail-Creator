using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace BTC_Blazor.Pages
{
    public partial class PictureDataBox
    {
        private PictureData pictureData;

        [Parameter]
        public PictureData PictureDataProp
        {
            get => pictureData;
            set
            {
                pictureData = value;
                UpdateData();
            }
        }

        public string FileName { get; set; }
        public Box CurrentBox { get; set; }
        public string Font { get; set; }
        public OutputType outPutType { get; set; }
        public Dictionary<Box, Rectangle> AvailableBoxes { get; set; }

        private void UpdateData()
        {
            FileName = PictureDataProp.FileName;
            CurrentBox = PictureDataProp.ParamForTextCreation.CurrentBox;
            Font = PictureDataProp.ParamForTextCreation.Font;
            outPutType = PictureDataProp.OutputType;
            AvailableBoxes = PictureDataProp.ParamForTextCreation.Boxes;
        }

    }

}


