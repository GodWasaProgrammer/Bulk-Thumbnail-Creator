using Bulk_Thumbnail_Creator;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class BTCGenerator
    {
        private bool _processing = false;
        private bool DisableMenu = false;

        [Inject]
        PictureDataService DataService { get; set; }
        private List<string> DownloadedVideosList = new List<string> { "Video 1", "video 2" };

        private string textInput = string.Empty;
        private string TextToPrint1 = "Good Ole Rambler try!";
        private string TextToPrint2 = "I've Taken Dunkirk! Onwards Men!";
        private string TextToPrint3 = "Tallyhoo Laddiooo";

        private async Task CallBTC()
        {
            StateHasChanged(); // tells UI to re-render

            List<string> ListOfTextToPrint = new List<string> { TextToPrint1, TextToPrint2, TextToPrint3 };
            await DataService.CreateInitialPictureArrayAsync(textInput, ListOfTextToPrint);
            StateHasChanged(); // tells UI to re-render
        }

        private List<string> DisplayList = new();

        private async void AwaitBTC()
        {
            DisableMenu = true;
            _processing = true;
            StateHasChanged();
            await CallBTC();
            _processing = false;
            StateHasChanged();
        }

        private void BuildVideosList()
        {
            DownloadedVideosList = Logic.DeSerializeXMLToListOfStrings(BTCSettings.PathToXMLListOfDownloadedVideos);

            foreach (var video in DownloadedVideosList) 
            {
                DisplayList.Add(Path.GetFileName(video));
            }

        }

        protected override void OnInitialized()
        {
            BuildVideosList();
        }

        public ImageDisplay imageDisplayref;

    }

}
