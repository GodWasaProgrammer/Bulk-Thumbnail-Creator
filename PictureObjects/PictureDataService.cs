using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

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

        public async Task CreateCustomPicture(PictureData CustomPictureData)
        {
            string url = string.Empty;

            PicDataServiceList = await Program.Process(ProductionType.CustomPicture, url, TextToPrint, CustomPictureData);
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
