namespace BulkThumbnailCreator.Interfaces;

public interface ISettings
{
    bool Mocking { get; set; }
    bool MakeMocking { get; set; }
    string OutputDir { get; set; }
    string TextAddedDir { get; set; }
    string YTDLPDir { get; set; }
    string YTDLOutPutDir { get; set; }
    string FfmpegDir { get; set; }
    string[] Files { get; set; }
    string DankMemeStashDir { get; set; }
    string[] Memes { get; set; }
    string PathToVideo { get; set; }
}
