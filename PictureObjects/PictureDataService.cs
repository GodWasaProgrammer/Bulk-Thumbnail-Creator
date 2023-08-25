using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Bulk_Thumbnail_Creator.PictureObjects
{
    public class PictureDataService
    {
        string Text1;
        string Text2;
        string Text3;
  
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

        public async Task CreateInitialPictureArrayAsync(string url, List<string> ListOfTextToPrint)
        {
            Text1 = ListOfTextToPrint[0];
            Text2 = ListOfTextToPrint[1];
            Text3 = ListOfTextToPrint[2];

            ProductionType ProdType = ProductionType.FrontPagePictureLineUp;
            PicDataServiceList = await Program.Process(ProdType, url, ListOfTextToPrint);
        }

        public async Task<List<string>> CreatePictureDataVariety(PictureData PicToVarietize)
        {
            string url = string.Empty;
            List<string> texts = new List<string>();
            texts.Add(Text1);
            texts.Add(Text2);
            texts.Add(Text3);

            ProductionType ProdType = ProductionType.VarietyList;

            PicDataServiceList = await Program.Process(ProdType, url, texts, PicToVarietize);

            List<string> ImageUrls = new List<string>();

            for (int i = 0; i < PicToVarietize.Varieties.Count; i++)
            {
                ImageUrls.Add(PicToVarietize.Varieties[i].OutPath);
            }

            return ImageUrls;
        }

        public void AddPictureData(PictureData pictureData)
        {
            PicDataServiceList.Add(pictureData);
        }

        public void ResetPictureDataList()
        {
            PicDataServiceList.Clear();
        }

        public void ResetOutPutList()
        {
            OutputFileServiceList.Clear();
        }

        public static void ClearBaseOutPutDirectories()
        {
            DirectoryInfo di = new DirectoryInfo(BTCSettings.TextAddedDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            DirectoryInfo di2 = new DirectoryInfo(BTCSettings.OutputDir);
            
            foreach(FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo dir in di2.GetDirectories())
            {
                dir.Delete(true); 
            }

            DirectoryInfo di3 = new DirectoryInfo(BTCSettings.YoutubeDLDir);

            foreach (FileInfo file in di3.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo dir in di3.GetDirectories())
            { 
                dir.Delete(true); 
            }

            // TODO clear subdirs also

        }

    }

}
