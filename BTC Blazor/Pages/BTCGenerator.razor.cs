﻿using Bulk_Thumbnail_Creator;
using Bulk_Thumbnail_Creator.PictureObjects;
using Bulk_Thumbnail_Creator.Serialization;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class BTCGenerator
    {
        private bool _processing = false;
        private bool DisableMenu = false;

        [Inject]
        PictureDataService DataService { get; set; }
        private List<string> DownloadedVideosList = new() { "Video 1", "video 2" };

        private string textInput = string.Empty;
        private bool passedNull = false;
        private string TextToPrint1 = "Good Ole Rambler try!";
        private string TextToPrint2 = "I've Taken Dunkirk! Onwards Men!";
        private string TextToPrint3 = "Tallyhoo Laddiooo";

        private async Task CallBTC(string url)
        {
            StateHasChanged(); // tells UI to re-render

            List<string> ListOfTextToPrint = new() { TextToPrint1, TextToPrint2, TextToPrint3 };
            await DataService.CreateInitialPictureArrayAsync(url, ListOfTextToPrint);
            StateHasChanged(); // tells UI to re-render
        }

        private List<string> DisplayList = new();

        private async void AwaitBTC()
        {
            string url = textInput;

            if (url == null || url == string.Empty)
            {
                passedNull = true;
            }
            else
            {
                passedNull = false;
                DisableMenu = true;
                _processing = true;
                StateHasChanged();
                await CallBTC(url);
                _processing = false;
                StateHasChanged();
            }

        }

        private void BuildVideosList()
        {
            DownloadedVideosList = Serializing.DeSerializeXMLToListOfStrings(BTCSettings.PathToXMLListOfDownloadedVideos);

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
