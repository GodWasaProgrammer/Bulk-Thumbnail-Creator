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
            SetPictureDataImageDisplayCorrelation(ImageUrl);
        }

        public void SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            foreach (var item in PictureDataService.PicDataServiceList)
            {

                if (Path.GetFileNameWithoutExtension(item.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                {
                    CurrentPagePictureData = new PictureData(item);
                }

                //if (CurrentPagePictureData == item)
                //{
                //    CurrentPagePictureData = new PictureData(item);
                //}

            }
            
        }

    }

}
