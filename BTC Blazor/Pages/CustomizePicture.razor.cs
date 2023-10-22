using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace BTC_Blazor.Pages
{
    public partial class CustomizePicture
    {
        public CustomizePicture()
        {

        }

        public PictureData CurrentPagePictureData { get; set; }

        public PictureData PicDataToCustomize = new();

        public Box PickedBox { get; set; }
        public List<string> InputText { get; set; }
        public Box MemeBox { get; set; }
        public List<string> AvailableFonts { get { return Directory.GetFiles("Fonts", "*.TTF*").ToList(); } }

        public string PickedFont { 
            get 
            {
                _PickedFont ??= CurrentPagePictureData.ParamForTextCreation.Font;

                return _PickedFont;
            }  
            set { _PickedFont = value; }
        }

        private string _PickedFont;

        [Parameter]
        public string ImageURL { get; set; }

        public async void CreateCustomPicDataObject()
        {
            OutputType jobtype = OutputType.Custom;
            PicDataToCustomize = await PicDataService.CreateCustomPicDataObject(CurrentPagePictureData, PickedFont, PickedBox, CurrentPagePictureData.Dankbox, BorderCLRItem.Hue, BorderCLRItem.Saturation, BorderCLRItem.Luminance, fillCLRItem.Hue, fillCLRItem.Saturation, fillCLRItem.Luminance, StrokeCLRItem.Hue, StrokeCLRItem.Saturation, StrokeCLRItem.Luminance, jobtype);
            ShowCustomPicture(PicDataToCustomize);
        }

        private void NavToCustomizePicture(string imageURL)
        {
           var currentdir = Directory.GetCurrentDirectory();

            imageURL = imageURL.Substring(currentdir.Length);

            navmanager.NavigateTo($"/CustomizePicture/{Uri.EscapeDataString(imageURL)}");
        }

        private void ShowCustomPicture(PictureData CustomPicture)
        {
            CurrentPagePictureData = new(CustomPicture);
            ImageURL = CurrentPagePictureData.OutPath; // just text data fetch
            NavToCustomizePicture(ImageURL); // trying to set the correlating image
            StateHasChanged();
            Logger.LogInformation("ShowCustomPicture Has been called in Blazors Component");
        }

        protected override void OnInitialized()
        {
            CurrentPagePictureData = PicDataService.SetPictureDataImageDisplayCorrelationForVarietyList(ImageURL);
            BorderCLRItem = CurrentPagePictureData.ParamForTextCreation.BorderColor;
            fillCLRItem = CurrentPagePictureData.ParamForTextCreation.FillColor;
            StrokeCLRItem = CurrentPagePictureData.ParamForTextCreation.StrokeColor;
        }

        private bool _processing = false;

        private List<ColorGroup> colorBoxes = Enum.GetValues(typeof(ColorGroup)).Cast<ColorGroup>().ToList();

        private MudColor colorvalue;

        private ColorGroup PickedGroup;

        // get value from picker
        private MudColor BorderColor;
        // set value and display it
        private ColorItem BorderCLRItem;

        // get value from picker
        private MudColor FillColor;
        // set value and display it
        private ColorItem fillCLRItem;

        // get value from picker
        private MudColor StrokeColor;
        // set value and display it
        private ColorItem StrokeCLRItem;

        private enum ColorGroup
        {
            FillColor,
            BorderColor,
            StrokeColor
        }

        private void SetColor(ColorGroup clrGRP)
        {
            if (clrGRP == ColorGroup.BorderColor)
            {
                BorderColor = colorvalue;
                BorderCLRItem.SetByHSL((float)BorderColor.H, (float)BorderColor.S, (float)BorderColor.L);
            }
            if (clrGRP == ColorGroup.FillColor)
            {
                FillColor = colorvalue;
                fillCLRItem.SetByHSL((float)FillColor.H, (float)FillColor.S, (float)FillColor.L);
            }
            if (clrGRP == ColorGroup.StrokeColor)
            {
                StrokeColor = colorvalue;
                StrokeCLRItem.SetByHSL((float)StrokeColor.H, (float)StrokeColor.S, (float)StrokeColor.L);
            }
            StateHasChanged();
        }

    }

}