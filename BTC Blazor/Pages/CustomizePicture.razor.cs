using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BTC_Blazor.Pages
{
    public partial class CustomizePicture
    {
        public PictureData CurrentPagePictureData { get; set; }

        [Parameter]
        public string ImageURL { get; set; }

        public CustomizePicture()
        {

        }

        protected override void OnInitialized()
        {
            SetPictureDataImageDisplayCorrelation(ImageURL);
        }

        public void SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            foreach (var item in PicDataService.PicDataServiceList)
            {

                // need to go one deeper insdide varietieis to correct the diff
                //

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
