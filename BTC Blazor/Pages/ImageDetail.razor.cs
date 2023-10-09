using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class ImageDetail
    {

        [Parameter]
        public PictureData CurrentPagePictureData { get; set; }

        [Parameter]
        public string ImageUrl { get; set; }

        protected override void OnInitialized()
        {
            CurrentPagePictureData = PicDataService.SetPictureDataImageDisplayCorrelation(ImageUrl);
        }

    }

}