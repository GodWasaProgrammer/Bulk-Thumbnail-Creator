using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class BTCGenerator
    {
        [Inject]
        PictureDataService DataService { get; set; }
        private bool isLoading = false;
        private string textInput = string.Empty;
        private string TextToPrint1 = "Good Ole Rambler try!";
        private string TextToPrint2 = "I've Taken Dunkirk! Onwards Men!";
        private string TextToPrint3 = "Tallyhoo Laddiooo";

        private async Task CallBTC()
        {
            List<string> ListOfTextToPrint = new List<string> { TextToPrint1, TextToPrint2, TextToPrint3 };
            await DataService.CreateInitialPictureArrayAsync(textInput, ListOfTextToPrint);
        }

    }

}
