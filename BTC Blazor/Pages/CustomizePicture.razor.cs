using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Microsoft.AspNetCore.Components;
using Bulk_Thumbnail_Creator.InterFaces;

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
            PicDataToCustomize = new(CurrentPagePictureData);
            PicDataToCustomize.ParamForTextCreation.CurrentBox = PickedBox;
            PicDataToCustomize.Dankbox = MemeBox;
            PicDataToCustomize.ParamForTextCreation.Font = PickedFont;
            PicDataToCustomize.ParamForTextCreation.BorderColor.SetByHSL(BorderCLRItem.Hue, BorderCLRItem.Saturation, BorderCLRItem.Luminance);
            PicDataToCustomize.ParamForTextCreation.FillColor.SetByHSL(fillCLRItem.Hue, fillCLRItem.Saturation, fillCLRItem.Luminance);
            PicDataToCustomize.ParamForTextCreation.StrokeColor.SetByHSL(StrokeCLRItem.Hue, StrokeCLRItem.Saturation, StrokeCLRItem.Luminance);
            PicDataToCustomize.OutPutType = OutputType.Custom;

            await PicDataService.CreateCustomPicture(PicDataToCustomize);
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
