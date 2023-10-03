using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class ImageDetail
    {
        public PictureData CurrentPagePictureData;

        [Parameter]
        public string ImageUrl { get; set; }

        protected override void OnInitialized()
        {
            CurrentPagePictureData = PicDataService.SetPictureDataImageDisplayCorrelation(ImageUrl);
        }

    }

}