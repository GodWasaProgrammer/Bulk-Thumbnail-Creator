using Bulk_Thumbnail_Creator;
using Bulk_Thumbnail_Creator.PictureObjects;

namespace BTC_Blazor.Pages
{
    public partial class ImageDisplay
    {
        // List of image URLs
        private List<string> imageUrls;

        // Load image URLs in the OnInitializedAsync lifecycle method
        protected override async Task OnInitializedAsync()
        {
            await UpdateImageUrls();

            TimerInterval = 5000;
            Timer = new System.Threading.Timer(async _ =>
            {
                await UpdateImageUrlsWithInvoke();
            }, null, TimerInterval, TimerInterval);
        }

        // Method to update the list of image URLs
        private async Task UpdateImageUrls()
        {
            string imagePath = BTCSettings.TextAddedDir;

            // Get the list of image files in the folder
            string[] imageFiles = Directory.GetFiles(imagePath, "*.png");

            // Initialize the image URLs list
            imageUrls = new List<string>();

            // Create the URLs for the images and add them to the list
            foreach (string imageFile in imageFiles)
            {
                string imageUrl = $"/{imageFile}";
                imageUrls.Add(imageUrl);
            }

            // Notify the component that the state has changed
            StateHasChanged();
        }

        // update the list of image URLs using InvokeAsync
        private async Task UpdateImageUrlsWithInvoke()
        {
            // Use InvokeAsync to switch to the Blazor UI thread before calling UpdateImageUrls
            await InvokeAsync(UpdateImageUrls);
        }

        // Timer properties
        private System.Threading.Timer Timer { get; set; }
        private int TimerInterval { get; set; }
    }
}

