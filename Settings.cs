// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Ignore Spelling: Ffmpeg Meme Memes Datas

namespace BulkThumbnailCreator
{
    public class Settings(ILogService logger, JobService jobService)
    {
        public static bool Mocking { get; set; } = false;
        public static bool MakeMocking { get; set; } = true;
        public ILogService LogService { get; set; } = logger;
        public JobService JobService { get; set; } = jobService;
        public string OutputDir { get; set; } = "output";
        public string TextAddedDir { get; set; } = "TextAdded";
        public string YTDLPDir { get; set; }
        public string YTDLOutPutDir { get; set; } = "YTDL";
        public string FfmpegDir { get; set; }
        public string[] Files { get; set; }
        public static string DankMemeStashDir { get; set; } = "DankMemeStash";
        public static string[] Memes { get; set; }
        public List<PictureData> PictureDatas { get; set; } = [];
        private const int MaxRGB = 256;
        public static int RGBMax { get { return MaxRGB; } }
        public List<string> ListOfText { get; set; } = [];
        public string PathToVideo { get; set; }
        public static List<string> DownloadedVideosList { get; set; } = [];
    }
}
