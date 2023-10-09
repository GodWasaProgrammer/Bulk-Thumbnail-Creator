﻿using Bulk_Thumbnail_Creator;

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
        }

        // Method to update the list of image URLs
        private async Task UpdateImageUrls()
        {
            string imagePath = BTCSettings.TextAddedDir;

            // Get the list of image files in the folder asynchronously
            string[] imageFiles = await Task.Run(() => Directory.GetFiles(imagePath, "*.png"));

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

        private bool IMGDetailClicked = false;


        private void ShowImageDetail(string imageUrl)
        {
            IMGDetailClicked = true;

            navmanager.NavigateTo($"/imagedetail/{Uri.EscapeDataString(imageUrl)}");
        }

    }

}