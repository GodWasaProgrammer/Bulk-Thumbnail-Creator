using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using Bulk_Thumbnail_Creator.InterFaces;
using MudBlazor.Utilities;

namespace BTC_Blazor.Pages
{
    public partial class CustomizePicture : IPictureData 
    {
        public string FileName { get => CurrentPagePictureData.FileName; set => throw new NotImplementedException(); }
        public string OutPath { get => CurrentPagePictureData.OutPath; set => throw new NotImplementedException(); }
        public ParamForTextCreation ParamForTextCreation { get => CurrentPagePictureData.ParamForTextCreation; set => throw new NotImplementedException(); }
        public List<PictureData> Varieties { get => CurrentPagePictureData.Varieties; set => throw new NotImplementedException(); }
        public OutputType OutPutType { get => CurrentPagePictureData.OutPutType; set => throw new NotImplementedException(); }
        public Box Dankbox { get => CurrentPagePictureData.Dankbox; set => throw new NotImplementedException(); }
        public string Meme { get => CurrentPagePictureData.Meme; set => value = Meme; }

        public PictureData CurrentPagePictureData { get; set; }
        

        public PictureData PicDataToCustomize = new();

        public Box PickedBox { get; set; }
        public List<string> InputText { get; set; }
        public Box MemeBox { get; set; }
        public List<string> AvailableFonts { get { return Directory.GetFiles("Fonts", "*.TTF*").ToList(); } }

        public string PickedFont { 
            get 
            {
                if (_PickedFont == null)
                {
                    _PickedFont = CurrentPagePictureData.ParamForTextCreation.Font;
                }

                return _PickedFont;
            }  
            set { _PickedFont = value; }
        }

        private string _PickedFont;

        [Parameter]
        public string ImageURL { get; set; }

        public CustomizePicture()
        {

        }

        public async void CreateCustomPicDataObject()
        {
            OutputType jobtype = OutputType.Custom;
            PicDataToCustomize = await PicDataService.CreateCustomPicDataObject(PicDataToCustomize, PickedFont, PickedBox, Dankbox, BorderCLRItem.Hue, BorderCLRItem.Saturation, BorderCLRItem.Luminance, fillCLRItem.Hue, fillCLRItem.Saturation, fillCLRItem.Luminance, StrokeCLRItem.Hue, StrokeCLRItem.Saturation, StrokeCLRItem.Luminance, jobtype);
            ShowCustomPicture(PicDataToCustomize);
        }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public PictureData PicData => throw new NotImplementedException();

        private void NavToCustomizePicture(string imageURL)
        {
           var currentdir = Directory.GetCurrentDirectory();

            imageURL = imageURL.Substring(currentdir.Length);

            NavigationManager.NavigateTo($"/CustomizePicture/{Uri.EscapeDataString(imageURL)}");
        }

        private void ShowCustomPicture(PictureData CustomPicture)
        {
            CurrentPagePictureData = new(CustomPicture);
            ImageURL = CurrentPagePictureData.OutPath; // just text data fetch
            NavToCustomizePicture(ImageURL); // trying to set the correlating image
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            PicDataService.SetPictureDataImageDisplayCorrelationForVarietyList(ImageURL);
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