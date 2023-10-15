using Bulk_Thumbnail_Creator;
using Bulk_Thumbnail_Creator.Serialization;
using Bulk_Thumbnail_Creator.Services;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class BTCGenerator
    {

        private List<string> DownloadedVideosList = new() { "Video 1", "video 2" };


        private List<string> DisplayList = new();

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

    }

}
