using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;

namespace Bulk_Thumbnail_Creator
{
	public class Program
	{
		public static async Task Main()
		{
			// YT-DL
			// declare instance of youtubeDL
			var ytdl = new YoutubeDL();

			// set paths
			ytdl.YoutubeDLPath = "..\\..\\yt-dlp.exe";
			ytdl.FFmpegPath = "..\\..\\ffmpeg.exe";
			ytdl.OutputFolder= "..\\..\\YTDL";
			//---------------------------------------------

			// downloads specified video from youtube
			var res = await ytdl.RunVideoDownload("https://www.youtube.com/watch?v=b627luXmC1E&t=");
			// the path of the downloaded file
			string path = res.Data;

			// sets BTC to run on the recently downloaded file
			BTCSettings.PathToVideo = path;

			// creates our 2 dirs to push out unedited thumbnails, and the edited thumbnails.
			Directory.CreateDirectory(BTCSettings.OutputDir);
			Directory.CreateDirectory(BTCSettings.TextAddedDir);

			BTCSettings.IntervalBetweenThumbnails = Logic.SplitMetaDataIntoInterValsForThumbNailCreation();

			// creates x images from a mediafile
			for (int i = 0; i < BTCSettings.NumberOfThumbnails; i++)
			{
				Logic.ExtractThumbnails(i);
				Logic.IncreaseInterval();
			}

			// loops foreach file in list of filepaths, generate some settings, return the settings, add em to our listofsettingsfortext.
			foreach (string fileName in BTCSettings.FileNames)
			{
				Logic.listOfSettingsForText.Add(Logic.GenerateLinearProgressionColorSettings());
				// Logic.listOfSettingsForText.Add(Logic.GenerateRandomColorSettings());
			}

			/// dev area
			Logic.TextAdder();
			Logic.MemeStashDirectories();

			//
			Logic.AddTextComposite();
		}

	}

}