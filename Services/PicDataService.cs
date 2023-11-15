﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using Bulk_Thumbnail_Creator.Interfaces;
using Bulk_Thumbnail_Creator.Serialization;

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
            PicDataServiceList = await Creator.Process(ProdType, url, ListOfTextToPrint);
            TextToPrint = ListOfTextToPrint;
        }

        public async Task<List<string>> CreatePictureDataVariety(PictureData PicToVarietize)
        {
            string url = string.Empty;

            ProductionType ProdType = ProductionType.VarietyList;

            PicDataServiceList = await Creator.Process(ProdType, url, TextToPrint,PicToVarietize);

            List<string> ImageUrls = new();

            for (int i = 0; i < PicToVarietize.Varieties.Count; i++)
            {
                ImageUrls.Add(PicToVarietize.Varieties[i].OutPath);
            }

            return ImageUrls;
        }

        public async Task<PictureData> CreateCustomPicDataObject(PictureData PicToCustomize, string PickedFont, BoxType PickedBox, float Borderhue, float Bordersat, float BorderLum, float FillCLRHue, float FillCLRSat, float FillCLRLum, float StrokeCLRHue, float StrokeCLRSat, float strokeCLRLum, OutputType JobType, ILogService logger)
        {
            PicToCustomize = new(PicToCustomize);

            for(int BoxParam = 0; BoxParam < PicToCustomize.BoxParameters.Count; BoxParam++)
            {

                PicToCustomize.BoxParameters[BoxParam].CurrentBox = PickedBox;
            // PicToCustomize.Dankbox = DankBox;
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

        public PictureData SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            PictureData PicData = new();

            foreach (var item in PicDataServiceList)
            {

                if (Path.GetFileNameWithoutExtension(item.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                {
                    PicData = new PictureData(item);
                    break;
                }
            }
            return PicData;
        }

        public PictureData SetPictureDataImageDisplayCorrelationForVarietyList(string imageUrl)
        {
            PictureData PicData = new();

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