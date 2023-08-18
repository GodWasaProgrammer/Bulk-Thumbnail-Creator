using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator.PictureObjects
{
    public class PictureDataService
    {
  
        public PictureDataService()
        {
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
            ProductionType ProdType = ProductionType.FrontPagePictureLineUp;
            PicDataServiceList = await Program.Process(ProdType, url, ListOfTextToPrint);
        }

        public async Task CreatePictureDataVariety(PictureData PicToVarietize)
        {
            ProductionType ProdType = ProductionType.VarietyList;
            PictureDataService = await Program.Process(ProdType,)
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

    }

}
