using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.Interfaces;
using Bulk_Thumbnail_Creator.PictureObjects;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bulk_Thumbnail_Creator.Services
{
    public class PicDataService
    {
        public PicDataService(ILogService _logger)
        {
            ClearBaseOutPutDirectories();
            PicDataServiceList = new List<PictureData>();
            OutputFileServiceList = new List<string>();
            Settings.LogService = _logger;
        }

        public List<PictureData> PicDataServiceList { get; set; } = new List<PictureData>();
        public List<string> OutputFileServiceList { get; set; } = new();
        public List<string> TextToPrint { get; set; } = new();

        public async Task CreateInitialPictureArrayAsync(string url, List<string> ListOfTextToPrint)
        {
            ProductionType ProdType = ProductionType.FrontPagePictureLineUp;
            TextToPrint = ListOfTextToPrint;

            if (Settings.Mocking == true)
            {
               PicDataServiceList = await Creator.MockProcess(ProdType, url, ListOfTextToPrint);
            }
            else
            {
                PicDataServiceList = await Creator.Process(ProdType, url, ListOfTextToPrint);
            }
        }

        public async Task<List<string>> CreatePictureDataVariety(PictureData PicToVarietize)
        {
            List<string> ImageUrls = new();

            if (Settings.Mocking == true)
            {
                PicDataServiceList = await Creator.MockProcess(ProductionType.VarietyList, string.Empty, TextToPrint, PicToVarietize);

                string parentfilename = Path.GetFileName(PicToVarietize.FileName);

                DirectoryInfo di = new DirectoryInfo(Settings.TextAddedDir);

                DirectoryInfo[] di2 = di.GetDirectories();

                foreach (DirectoryInfo di3 in di2)
                {

                }

                string varietyof = "variety of";
                string ConcatenatedString = $"{Settings.TextAddedDir}/{varietyof} {parentfilename}";
                string[] ArrayOfFilePaths = Directory.GetFiles(ConcatenatedString, "*.png");

                foreach (string filepath in ArrayOfFilePaths)
                {
                    string imageurl = $"/{filepath}"; // convert to URL
                    ImageUrls.Add(imageurl);
                }

            }
            else
            {
                string url = string.Empty;

                ProductionType ProdType = ProductionType.VarietyList;

                PicDataServiceList = await Creator.Process(ProdType, url, TextToPrint, PicToVarietize);

                string parentfilename = Path.GetFileName(PicToVarietize.FileName);
                string varietyof = "variety of";
                string ConcatenatedString = $"{Settings.TextAddedDir}/{varietyof} {parentfilename}";
                string[] ArrayOfFilePaths = Directory.GetFiles(ConcatenatedString, "*.png");

                foreach (string filepath in ArrayOfFilePaths)
                {
                    string imageurl = $"/{filepath}"; // convert to URL
                    ImageUrls.Add(imageurl);
                }

            }

            return ImageUrls;
        }

        public async Task<PictureData> CreateCustomPicDataObject(PictureData PicToCustomize, string PickedFont, BoxType PickedBox, float Borderhue, float Bordersat, float BorderLum, float FillCLRHue, float FillCLRSat, float FillCLRLum, float StrokeCLRHue, float StrokeCLRSat, float strokeCLRLum, OutputType JobType)
        {
            PicToCustomize = new(PicToCustomize);

            for (int BoxParam = 0; BoxParam < PicToCustomize.BoxParameters.Count; BoxParam++)
            {
                PicToCustomize.BoxParameters[BoxParam].CurrentBox.Type = PickedBox;
                PicToCustomize.BoxParameters[BoxParam].Font = PickedFont;
                PicToCustomize.BoxParameters[BoxParam].BorderColor.SetByHSL(Borderhue, Bordersat, BorderLum);
                PicToCustomize.BoxParameters[BoxParam].FillColor.SetByHSL(FillCLRHue, FillCLRSat, FillCLRLum);
                PicToCustomize.BoxParameters[BoxParam].StrokeColor.SetByHSL(StrokeCLRHue, StrokeCLRSat, strokeCLRLum);
                PicToCustomize.OutPutType = JobType;
            }

            string url = string.Empty;
            PicDataServiceList = await Creator.Process(ProductionType.CustomPicture, url, TextToPrint, PicToCustomize);

            return PicToCustomize;
        }


        public Task<PictureData> SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            PictureData PicData = new();

            if(Settings.Mocking == true)
            {
                string DirToMockPicture = Path.Combine("..", "Mocking","FrontpagePictureLineUp", $"{Settings.TextAddedDir}");

                DirectoryInfo di = new DirectoryInfo(DirToMockPicture);

                DirectoryInfo[] di2 = di.GetDirectories();

                string MockCorrelation = "";

                foreach (DirectoryInfo directory in di2)
                {
                    MockCorrelation = Path.GetFileName(directory.FullName);
                }

                foreach (var item in PicDataServiceList)
                {
                    string NumberOfPicture = Path.GetFileNameWithoutExtension(item.FileName);

                    string NumberOfPicture2 = Path.GetFileNameWithoutExtension(MockCorrelation);

                    string varof = "variety of ";

                    NumberOfPicture2 = NumberOfPicture2.Remove(0, varof.Length);

                    if (NumberOfPicture == NumberOfPicture2)
                    {
                        PicData = new PictureData(item);
                        break;
                    }
                }

            }
            else
            {
                foreach (var item in PicDataServiceList)
                {

                    if (Path.GetFileNameWithoutExtension(item.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                    {
                        PicData = new PictureData(item);
                        break;
                    }
                }
            }

            return Task.FromResult(PicData);
        }

        public PictureData SetPictureDataImageDisplayCorrelationForVarietyList(string imageUrl)
        {
            PictureData PicData = new();

            if (Settings.Mocking == true)
            {
                DirectoryInfo di = new DirectoryInfo(Settings.TextAddedDir);

                DirectoryInfo[] di2 = di.GetDirectories();

                string MockCorrelation = "";

                foreach (DirectoryInfo directory in di2)
                {
                    MockCorrelation = Path.GetFileName(directory.FullName);
                }

                foreach (var item in PicDataServiceList)
                {
                    foreach (PictureData variety in item.Varieties)
                    {
                            PicData = new PictureData(variety);
                            break;
                    }

                }

            }
            else
            {
                foreach (var item in PicDataServiceList)
                {
                    foreach (PictureData variety in item.Varieties)
                    {
                        if (Path.GetFileNameWithoutExtension(variety.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                        {
                            PicData = new PictureData(variety);
                            break;
                        }

                    }

                }

            }
            return PicData;
        }

        public static void SaveWork()
        {
            using (StreamWriter streamWriter = new("Picturedatas.xml"))
            {
                foreach (var PictureData in Settings.PictureDatas)
                {
                    //Serializing.SerializePictureData(streamWriter, PictureData);

                }
                // logger.LogInformation("PictureDatas.xml Serialized from PictureData");
            }
        }

        public static void ClearBaseOutPutDirectories()
        {
            DirectoryInfo di = new(Settings.TextAddedDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // TODO : for whatever reason these resources are not free for deletion
            DirectoryInfo di2 = new(Settings.OutputDir);

            foreach (FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di2.GetDirectories())
            {
                dir.Delete(true);
            }

        }

    }

}