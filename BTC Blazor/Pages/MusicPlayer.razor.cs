using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection.Metadata;

namespace BTC_Blazor.Pages
{
    public partial class MusicPlayer
    {
        private string musicUrl = "song.mp3";

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                // Play the audio when the component is first rendered
                await JSRuntime.InvokeVoidAsync("playAudio");
            }

        }

    }

}

