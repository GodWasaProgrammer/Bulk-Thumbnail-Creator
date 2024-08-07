﻿namespace BulkThumbnailCreator.Interfaces;

public interface ISettings
{
    bool Mocking { get; set; }
    bool MakeMocking { get; set; }
    ILogService LogService { get; set; }
    JobService JobService { get; set; }
    string OutputDir { get; set; }
    string TextAddedDir { get; set; }
    string YTDLPDir { get; set; }
    string YTDLOutPutDir { get; set; }
    string FfmpegDir { get; set; }
    string[] Files { get; set; }
    string DankMemeStashDir { get; set; }
    string[] Memes { get; set; }
    List<PictureData> PictureDatas { get; set; }
    List<string> ListOfText { get; set; }
    string PathToVideo { get; set; }
    List<string> DownloadedVideosList { get; set; }
}
