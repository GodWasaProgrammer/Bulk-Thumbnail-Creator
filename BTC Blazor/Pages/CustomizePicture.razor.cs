using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class CustomizePicture
    {
        public PictureData CurrentPagePictureData { get; set; }

        public PictureData PicDataToCustomize = new PictureData();

        public Box CustomPickedBox { get; set; }
        public List<string> InputText { get; set; }
        public Box MemeBox { get; set; }
        public List<string> AvailableFonts { get { return Directory.GetFiles("Fonts").ToList(); } }
        public string PickedFont { get; set; }

        [Parameter]
        public string ImageURL { get; set; }

        public CustomizePicture()
        {

        }

        public void CreateCustomPicDataObject()
        {
            PicDataToCustomize = new(CurrentPagePictureData);
            PicDataToCustomize.ParamForTextCreation.CurrentBox = CustomPickedBox;
            PicDataToCustomize.Dankbox = MemeBox;
            PicDataToCustomize.ParamForTextCreation.Font = PickedFont;
        }

        protected override void OnInitialized()
        {
            SetPictureDataImageDisplayCorrelation(ImageURL);

        }

        private string GetColorValue(ColorGroup colorGroup, ColorProperty property)

        {
            switch (colorGroup)
            {
                case ColorGroup.FillColor:
                    return property switch
                    {
                        ColorProperty.Hue => CurrentPagePictureData.ParamForTextCreation.FillColor.Hue.ToString(),
                        ColorProperty.Saturation => CurrentPagePictureData.ParamForTextCreation.FillColor.Saturation.ToString(),
                        ColorProperty.Luminance => CurrentPagePictureData.ParamForTextCreation.FillColor.Luminance.ToString(),
                        _ => string.Empty,
                    };
                case ColorGroup.BorderColor:
                    return property switch
                    {
                        ColorProperty.Hue => CurrentPagePictureData.ParamForTextCreation.BorderColor.Hue.ToString(),
                        ColorProperty.Saturation => CurrentPagePictureData.ParamForTextCreation.BorderColor.Saturation.ToString(),
                        ColorProperty.Luminance => CurrentPagePictureData.ParamForTextCreation.BorderColor.Luminance.ToString(),
                        _ => string.Empty,
                    };
                case ColorGroup.StrokeColor:
                    return property switch
                    {
                        ColorProperty.Hue => CurrentPagePictureData.ParamForTextCreation.StrokeColor.Hue.ToString(),
                        ColorProperty.Saturation => CurrentPagePictureData.ParamForTextCreation.StrokeColor.Saturation.ToString(),
                        ColorProperty.Luminance => CurrentPagePictureData.ParamForTextCreation.StrokeColor.Luminance.ToString(),
                        _ => string.Empty,
                    };
                default:
                    return string.Empty;
            }

        }

        public void SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            foreach (var item in PicDataService.PicDataServiceList)
            {
                foreach (PictureData variety in item.Varieties) 
                {
                    if (Path.GetFileNameWithoutExtension(variety.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                    {
                        CurrentPagePictureData = new PictureData(variety);
                        break;
                    }

                }

            }

        }

    }

}
