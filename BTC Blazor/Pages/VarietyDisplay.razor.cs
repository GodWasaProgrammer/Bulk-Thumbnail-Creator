using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class VarietyDisplay
    {
        public List<string> imageUrls;
        // public NavigationManager navigationManager {get ;set;}

        public VarietyDisplay() 
        {
            imageUrls = new List<string>();
        }

        [Parameter]
        public PictureData ParentPictureProp { get; set;}

    }

}
