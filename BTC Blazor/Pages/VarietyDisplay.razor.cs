using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class VarietyDisplay
    {

        // List of image URLs
        private List<string> imageUrls;


        [Parameter]
        public PictureData ParentPictureProp { get; set; }

        public void SetPictureVarietyContent(PictureData MainIncoming)
        {
            for (int Data = 0; Data < PicDataService.PicDataServiceList.Count; Data++)
            {
                for (int VarietyData = 0; VarietyData < PicDataService.PicDataServiceList[Data].Varieties.Count; VarietyData++) 
                {

                }

            }

        }

    }

}
