using ImageMagick;
using System.Collections.Generic;

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


		private static Gravity _PositionOfText = Gravity.Southwest;
		public static Gravity PositionOfText { get { return _PositionOfText; } set { _PositionOfText = value; } }


		private static byte _RisingColorRedRGB = 0;
		public static byte RisingColorRedRGB { get { return _RisingColorRedRGB; } set { _RisingColorRedRGB = value; } }


		private static byte _FallingColorGreenRGB = 175;
		public static byte FallingColorGreenRGB { get { return _FallingColorGreenRGB; } set { _FallingColorGreenRGB = value; } }


		private static byte _FallingColorBlueRGB = 135;
		public static byte FallingColorBlueRGB { get { return _FallingColorBlueRGB; } set { _FallingColorBlueRGB = value; } }


		private static string _TextToAdd = "Bulk Thumbnail Creator";
		public static string TextToAdd { get { return _TextToAdd; } set { _TextToAdd = value; } }


		private const int _MaxRGB = 256;
		public static int MaxRGB { get { return _MaxRGB; } }


		private static List<string> _ListOfText = new List<string>();
		public static List<string> ListOfText { get { return _ListOfText; } set { _ListOfText = value; } }


		// path to the video to take thumbnails from
		private static string _PathToVideo;
		public static string PathToVideo { get { return _PathToVideo; } set { _PathToVideo = value; } }


		// the number of seconds between each thumbnail
		private static int _IntervalBetweenThumbnails;
		public static int IntervalBetweenThumbnails { get { return _IntervalBetweenThumbnails; } set { _IntervalBetweenThumbnails = value; } }


		// filenames of our outputted thumbnails
		private static string[] _Filenames;
		public static string[] FileNames { get { return _Filenames; } set { _Filenames = value; } }


		// an array of file paths of the dankmemestashfolder
		private static string[] _MemeStashPaths;
		public static string[] MemeStashFilePaths { get { return _MemeStashPaths; } set { _MemeStashPaths = value; } }


		private static List<string> _DownloadedVideosList = new List<string>();
		public static List<string> DownloadedVideosList { get { return _DownloadedVideosList; } set { _DownloadedVideosList = value; } }


		private static readonly string _PathToXMLListOfDownloadedVideos = "ListOfDownloadedVideos.xml";
		public static string PathToXMLListOfDownloadedVideos { get { return _PathToXMLListOfDownloadedVideos; } }

	}

}
