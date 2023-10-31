﻿using System.Collections.Generic;
using Bulk_Thumbnail_Creator.PictureObjects;
using Services.Interfaces;
using Services;

namespace Bulk_Thumbnail_Creator
{
    public class Settings
	{
		public static ILogService LogService { get; set; }
		
        /// <summary>
        /// The Dir where FFmpeg outputs raw images from the specified video
        /// </summary>
        private static string _outputDir = "output";
		public static string OutputDir { get { return _outputDir; } set { _outputDir = value; } }

		/// <summary>
		/// The Dir where images that have text added(compositioned) to it goes
		/// </summary>
		private static string _TextAddedDir = "text added";
		public static string TextAddedDir { get { return _TextAddedDir; } set { _TextAddedDir = value; } }

		// Location of YTDLP executable
		private static string _YTDLPDir;
		public static string YTDLPDir { get { return _YTDLPDir; } set { _YTDLPDir = value; } }

		/// <summary>
		/// The Dir where videos are downloaded
		/// </summary>
		private static string _YTDLOutPutDir = "YTDL";
		public static string YTDLOutPutDir { get { return _YTDLOutPutDir; } set { _YTDLOutPutDir = value; } }

		/// <summary>
		/// Directory where FFmpeg exe is located
		/// </summary>
		private static string _FfmpegDir;
		public static string FfmpegDir { get { return _FfmpegDir; } set { _FfmpegDir = value; } }
		
		/// <summary>
		/// String Array of the Filenames generated by ffmpeg
		/// </summary>
		private static string[] files;
		public static string[] Files { get { return files; } set { files = value; } }

		/// <summary>
		/// Where the available memes are stashed
		/// </summary>
		private static string _DankMemeStashDir = "DankMemeStash"; 
		public static string DankMemeStashDir { get { return _DankMemeStashDir; } set { _DankMemeStashDir = value; } }

		/// <summary>
		/// A string array of the available memes from the DankMemeStash
		/// </summary>
		private static string[] _Memes;
		public static string[] Memes {get { return _Memes;} set { _Memes = value; } }
		
		/// <summary>
		/// List of PictureDataObject which is a complete recipe to output an 
		/// image(or output an image again after the fact)
		/// </summary>
		private static List<PictureData> _PictureDatas = new();
		public static List<PictureData> PictureDatas { get { return _PictureDatas; } set { _PictureDatas = value; } }


		private static List<string> _DiscardedBecauseTooMuchFacefiles = new();
		public static List<string> DiscardedBecauseTooMuchFacePictureData {get { return _DiscardedBecauseTooMuchFacefiles; }set { _DiscardedBecauseTooMuchFacefiles = value; } }


		/// <summary>
		/// Link to the provided Youtube Link
		/// </summary>
		private static string _YoutubeLink;
		public static string YoutubeLink { get { return _YoutubeLink; } set { _YoutubeLink = value; } }


		private static string _TextToAdd = "Bulk Thumbnail Creator";
		public static string TextToAdd { get { return _TextToAdd; } set { _TextToAdd = value; } }

		/// <summary>
		/// Exists only to limit randomization to a 255 correct RGB end value
		/// </summary>
		private const int _MaxRGB = 256;
		public static int MaxRGB { get { return _MaxRGB; } }

		/// <summary>
		/// Where your output Text strings are located
		/// </summary>
		private static List<string> _ListOfText = new();
		public static List<string> ListOfText { get { return _ListOfText; } set { _ListOfText = value; } }


		/// <summary>
		/// path to the video to take thumbnails from
		/// </summary>
		private static string _PathToVideo;
		public static string PathToVideo { get { return _PathToVideo; } set { _PathToVideo = value; } }

		/// <summary>
		/// A list of the Videos that have previously been downloaded
		/// </summary>
		private static List<string> _DownloadedVideosList = new();
		public static List<string> DownloadedVideosList { get { return _DownloadedVideosList; } set { _DownloadedVideosList = value; } }

		/// <summary>
		/// Your XML of downloadedvideos
		/// </summary>
		private static readonly string _PathToXMLListOfDownloadedVideos = "ListOfDownloadedVideos.xml";
		public static string PathToXMLListOfDownloadedVideos { get { return _PathToXMLListOfDownloadedVideos; } }
	}

}