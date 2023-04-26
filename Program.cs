using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;
using FaceONNX;
using System.Drawing;
using System;
using UMapx.Visualization;

namespace Bulk_Thumbnail_Creator
{
	public class Program
	{
		public static async Task Main()
		{
			// YT-DL
			// declare instance of youtubeDL
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "..\\..\\ffmpeg.exe",
				OutputFolder = "YTDL"
			};
			//---------------------------------------------

			BTCSettings.YoutubeLink = "https://www.youtube.com/watch?v=tqBZuxid-XU&t=12s";
			// downloads specified video from youtube if it does not already exist.

			if (!File.Exists(BTCSettings.YoutubeLink))
			{
				var res = await ytdl.RunVideoDownload(url: BTCSettings.YoutubeLink);
				// sets BTC to run on the recently downloaded file res.data is the returned path.
				BTCSettings.PathToVideo = res.Data;
			};
			
			// creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
			Directory.CreateDirectory(BTCSettings.OutputDir);
			Directory.CreateDirectory(BTCSettings.TextAddedDir);
			Directory.CreateDirectory(BTCSettings.YoutubeDLDir);

			BTCSettings.IntervalBetweenThumbnails = Logic.SplitMetaDataIntoInterValsForThumbNailCreation();

			// creates x images from a mediafile
			for (int i = 0; i < BTCSettings.NumberOfThumbnails; i++)
			{
				Logic.ExtractThumbnails(i);
				Logic.IncreaseInterval();
			}

			// loops foreach file in list of filepaths, generate some settings, return the settings, add em to our listofsettingsfortext.
			for (int i = 0; i < BTCSettings.FileNames.Count; i++)
			{
				// if integer under the number of thumbnails split by 2
				if (i < BTCSettings.NumberOfThumbnails / 2 )
				{
					Logic.ListOfSettingsForText.Add(Logic.GenerateLinearProgressionColorSettings());
				}
				else
				{
					Logic.ListOfSettingsForText.Add(Logic.GenerateRandomColorSettings());
				}

			}

			Logic.TextAdder(BTCSettings.TextToAdd);
			// Logic.MemeStashDirectories(); not used at the moment but fully functional
			Logic.AddTextComposite(BTCSettings.PositionOfText);
		}

	}

}