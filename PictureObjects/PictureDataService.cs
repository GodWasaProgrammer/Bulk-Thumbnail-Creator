﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Bulk_Thumbnail_Creator.Enums;
using Microsoft.AspNetCore.Components;

namespace Bulk_Thumbnail_Creator.PictureObjects
{
    public class PictureDataService
    {
        public PictureDataService()
        {
            ClearBaseOutPutDirectories();
            PicDataServiceList = new List<PictureData>();
            OutputFileServiceList = new List<string>();
        }

        public PictureDataService(List<PictureData> pictureDatas)
        {
            PicDataServiceList = pictureDatas;
        }

        public List<PictureData> PicDataServiceList { get; set; } = new List<PictureData>();
        public List<string> OutputFileServiceList { get; set; } = new();
        public List<string> TextToPrint { get; set; } = new();

        public async Task CreateInitialPictureArrayAsync(string url, List<string> ListOfTextToPrint)
        {
            ProductionType ProdType = ProductionType.FrontPagePictureLineUp;
            PicDataServiceList = await Program.Process(ProdType, url, ListOfTextToPrint);
            TextToPrint = ListOfTextToPrint;
        }

        public async Task<List<string>> CreatePictureDataVariety(PictureData PicToVarietize)
        {
            string url = string.Empty;

            ProductionType ProdType = ProductionType.VarietyList;

            PicDataServiceList = await Program.Process(ProdType, url, TextToPrint, PicToVarietize);

            List<string> ImageUrls = new();

            for (int i = 0; i < PicToVarietize.Varieties.Count; i++)
            {
                ImageUrls.Add(PicToVarietize.Varieties[i].OutPath);
            }

            return ImageUrls;
        }

        public async Task<PictureData> CreateCustomPicDataObject(PictureData PicToCustomize, string PickedFont, Box PickedBox, Box DankBox, float Borderhue, float Bordersat, float BorderLum, float FillCLRHue, float FillCLRSat, float FillCLRLum, float StrokeCLRHue, float StrokeCLRSat, float strokeCLRLum, OutputType JobType)
        {
            PicToCustomize = new(PicToCustomize);
            PicToCustomize.ParamForTextCreation.CurrentBox = PickedBox;
            PicToCustomize.Dankbox = DankBox;
            PicToCustomize.ParamForTextCreation.Font = PickedFont;
            PicToCustomize.ParamForTextCreation.BorderColor.SetByHSL(Borderhue, Bordersat, BorderLum);
            PicToCustomize.ParamForTextCreation.FillColor.SetByHSL(FillCLRHue, FillCLRSat, FillCLRLum);
            PicToCustomize.ParamForTextCreation.StrokeColor.SetByHSL(StrokeCLRHue, StrokeCLRSat, strokeCLRLum);
            PicToCustomize.OutPutType = JobType;

            string url = string.Empty;
            PicDataServiceList = await Program.Process(ProductionType.CustomPicture, url, TextToPrint, PicToCustomize);

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

        public static void ClearBaseOutPutDirectories()
        {
            DirectoryInfo di = new(BTCSettings.TextAddedDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // TODO : for whatever reason these resources are not free for deletion
            DirectoryInfo di2 = new(BTCSettings.OutputDir);
            
            foreach(FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo dir in di2.GetDirectories())
            {
                dir.Delete(true); 
            }

        }

    }

}
