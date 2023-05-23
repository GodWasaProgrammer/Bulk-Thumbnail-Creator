using System.Collections.Generic;
using System.Drawing;

namespace Bulk_Thumbnail_Creator
{

	public class BTCSettings
	{
		// directories to create
		// 

		private static string _outputDir = "output";
		public static string OutputDir { get { return _outputDir; } set { _outputDir = value; } }


		private static string _TextAddedDir = "text added";
		public static string TextAddedDir { get { return _TextAddedDir; } set { _TextAddedDir = value; } }


		private static string _FaceDetectionDir = "face Detected";
		public static string FaceDetectionDir { get { return _FaceDetectionDir; } set { _FaceDetectionDir = value; } }


		private static string _YoutubeDLDir = "YTDL";
		public static string YoutubeDLDir { get { return _YoutubeDLDir; } set { _YoutubeDLDir = value; } }


		private static string _YoutubeLink;
		public static string YoutubeLink { get { return _YoutubeLink; } set { _YoutubeLink = value; } }


		private static string _TextToAdd = "Bulk Thumbnail Creator";
		public static string TextToAdd { get { return _TextToAdd; } set { _TextToAdd = value; } }


		private const int _MaxRGB = 256;
		public static int MaxRGB { get { return _MaxRGB; } }


		private static List<string> _ListOfText = new List<string>();
		public static List<string> ListOfText { get { return _ListOfText; } set { _ListOfText = value; } }


		// path to the video to take thumbnails from
		private static string _PathToVideo;
		public static string PathToVideo { get { return _PathToVideo; } set { _PathToVideo = value; } }

		private static List<string> _DownloadedVideosList = new List<string>();
		public static List<string> DownloadedVideosList { get { return _DownloadedVideosList; } set { _DownloadedVideosList = value; } }


		private static readonly string _PathToXMLListOfDownloadedVideos = "ListOfDownloadedVideos.xml";
		public static string PathToXMLListOfDownloadedVideos { get { return _PathToXMLListOfDownloadedVideos; } }
	}

}
