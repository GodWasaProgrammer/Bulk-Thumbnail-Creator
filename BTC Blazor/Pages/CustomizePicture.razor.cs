using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;

namespace BTC_Blazor.Pages
{
    public partial class CustomizePicture
    {
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
            PicDataToCustomize = new(CurrentPagePictureData);
            PicDataToCustomize.ParamForTextCreation.CurrentBox = PickedBox;
            PicDataToCustomize.Dankbox = MemeBox;
            PicDataToCustomize.ParamForTextCreation.Font = PickedFont;
            PicDataToCustomize.ParamForTextCreation.BorderColor.SetByHSL(BorderCLRItem.Hue, BorderCLRItem.Saturation, BorderCLRItem.Luminance);
            PicDataToCustomize.ParamForTextCreation.FillColor.SetByHSL(fillCLRItem.Hue, fillCLRItem.Saturation, fillCLRItem.Luminance);
            PicDataToCustomize.ParamForTextCreation.StrokeColor.SetByHSL(StrokeCLRItem.Hue, StrokeCLRItem.Saturation, StrokeCLRItem.Luminance);
            PicDataToCustomize.OutputType = OutputType.Custom;

            await PicDataService.CreateCustomPicture(PicDataToCustomize);
            ShowCustomPicture(PicDataToCustomize);
        }

        [Inject]
        NavigationManager NavigationManager { get; set; }

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
            SetPictureDataImageDisplayCorrelation(ImageURL);
            initializeColorItems();
        }

        public void SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            foreach (var item in PicDataService.PicDataServiceList)
            {
                foreach (PictureData variety in item.Varieties)
                {
                    if (Path.GetFileNameWithoutExtension(variety.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                    {
                        CurrentPagePictureData = new PictureData(variety);
                        break;
                    }

                }

            }

        }

    }

}
