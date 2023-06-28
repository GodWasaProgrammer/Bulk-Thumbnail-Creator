﻿using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YoutubeDLSharp;

namespace Bulk_Thumbnail_Creator
{
	internal class Logic
	{
		private static readonly Random colorRandom = new Random();

		/// <summary>
		/// Generates random colors in bytes
		/// </summary>
		/// <returns>returns a MagickColor Object which is RGB</returns>
		internal static MagickColor RandomizeColor()
		{
			byte pickedColorRedRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			byte pickedColorGreenRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			byte pickedColorBlueRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);

			MagickColor colorRNGPicked;

			colorRNGPicked = MagickColor.FromRgb(pickedColorRedRGB, pickedColorGreenRGB, pickedColorBlueRGB);

			return colorRNGPicked;
		}

		static float hueFillColor = 0F;
		static readonly float saturationFillColor = 1F;
		static readonly float lightnessFillColor = 0.50F;

		static float hueStrokeColor = 125F;
		static readonly float saturationStrokeColor = 1F;
		static readonly float lightnessStrokeColor = 0.50F;

		static float hueBorderColor = 28F;
		static readonly float saturationBorderColor = 1F;
		static readonly float lightnessBorderColor = 0.50F;

		/// <summary>
		/// Allows you to "spin" the HSL "globe" to "invert" colors
		/// </summary>
		/// <param name="inputHue">The Hue input Value to invert</param>
		/// <returns>the inverted hue value spun 180 degrees(float)</returns>
		public static float ColorWheelSpinner(float inputHue)
		{
			float fullSpin = 180F;

			if (inputHue < 180F)
			{
				inputHue += fullSpin;
			}
			else
			{
				inputHue -= fullSpin;
			}

			return inputHue;
		}

		/// <summary>
		/// Generates Color output to be used in a PictureData Object to generate text colors
		/// </summary>
		/// <param name="InputParameter">ParamForTextcreation Object to Generate Colors for</param>
		/// <param name="currentelement">the current index of the object being passed</param>
		/// <returns>returns the ParamForTextCreation object with the modified Color Values</returns>
		public static ParamForTextCreation DecideColorGeneration(ParamForTextCreation InputParameter, int currentelement)
		{
			const float maxHueValue = 360F;
			const float incrementalColor = 12.5F;
			const float resetFromMaxToMin = 0F;

			if (currentelement > 0)
			{
				hueFillColor += incrementalColor;

				hueStrokeColor = ColorWheelSpinner(hueFillColor);
				hueBorderColor = ColorWheelSpinner(hueFillColor);
				
				if (hueFillColor > maxHueValue)
				{
					hueFillColor = resetFromMaxToMin;
				}

				InputParameter.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);

				InputParameter.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);

				InputParameter.BorderColor.SetByHSL(hueBorderColor, saturationBorderColor, lightnessBorderColor);
			}
			else
			{
				InputParameter.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);

				InputParameter.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);

				InputParameter.BorderColor.SetByHSL(hueBorderColor, saturationBorderColor, lightnessBorderColor);
			}

			return InputParameter;
		}

		/// <summary>
		/// Take width, calculate the fontsize for your output
		/// </summary>
		/// <param name="Height"></param>
		public static int CalculateFontSize(int Height)
		{
			// this is 8 because it achieves a good ratio of text to the height of the picture (determined empirically)
			const int NumberToSplitBy = 8;

			int FontSize = Height / NumberToSplitBy;

			return FontSize;
		}

		static readonly XmlSerializer serializer = new XmlSerializer(typeof(List<string>));

		/// <summary>
		/// General DeSerializer for List of Strings
		/// </summary>
		/// <param name="pathtoXMLToDeSerialize">The XML to be deserialized</param>
		/// <returns>The Deserialized List of Strings</returns>
		public static List<string> DeSerializeXMLToListOfStrings(string pathtoXMLToDeSerialize)
		{
			List<string> ListofStringsToDeSerialize = BTCSettings.DownloadedVideosList;

			if (File.Exists(pathtoXMLToDeSerialize))
			{
				using (FileStream file = File.OpenRead(pathtoXMLToDeSerialize))
				{
					ListofStringsToDeSerialize = (List<string>)serializer.Deserialize(file);
				}

			}
			return ListofStringsToDeSerialize;
		}

		/// <summary>
		///  General Serializer for list of strings
		/// </summary>
		/// <param name="PathToXML">Your Path to the XML to be written to</param>
		/// <param name="ListOfStringsToSerialize">The List of Strings to Serialize</param>
		public static void SerializeListOfStringsToXML(string PathToXML, List<string> ListOfStringsToSerialize)
		{
			using (FileStream file = File.Create(PathToXML))
			{
				serializer.Serialize(file, ListOfStringsToSerialize);
			}

		}

		/// <summary>
		/// Instantiates a YoutubeDL and downloads the specified string
		/// </summary>
		/// <param name="URL">the specified link URL to download</param>
		/// <returns>returns the path to the downloaded video</returns>
		public static async Task<string> YouTubeDL(string URL)
		{
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "YTDL/ffmpeg.exe",
				OutputFolder = "YTDL"
			};

			// downloads specified video from youtube if it does not already exist.
			BTCSettings.YoutubeLink = URL;
			var res = await ytdl.RunVideoDownload(url: BTCSettings.YoutubeLink);
			await Console.Out.WriteLineAsync("Download Success:" + res.Success.ToString());

			// sets BTC to run on the recently downloaded file res.data is the returned path.
			return res.Data;
		}

		/// <summary>
		/// Generates MagickReadSettings with completely randomized color values for Fill/stroke/border colors
		/// ALso generates the other necessary settings for ImageMagick necessary to put text on the screenshots
		/// </summary>
		/// <returns>returns the randomized MagickReadSettings object</returns>
		public static MagickReadSettings GenerateRandomColorSettings()
		{
			MagickReadSettings settingsTextRandom = new MagickReadSettings
			{
				Font = "italic",
				FillColor = RandomizeColor(),
				StrokeColor = RandomizeColor(),
				BorderColor = RandomizeColor(),
				FontStyle = FontStyleType.Bold,
				StrokeAntiAlias = true,
				StrokeWidth = 6,
				//FontPointsize = 100,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				Width = 1700, // width of text box
			};

			return settingsTextRandom;
		}

		/// <summary>
		/// Picks a random font from the provided font folder
		/// </summary>
		/// <returns></returns>
		public static string PickRandomFont()
		{
			var fontNames = Directory.GetFiles("Fonts", "*.TTF*");

			Random randompicker = new Random();

			int randommax = fontNames.Length;

			int fontChosen = randompicker.Next(randommax);

			return fontNames[fontChosen].ToString();
		}

		/// <summary>
		/// Generates MagickReadSettings to be used in a PicturedataObject to decide how text will look
		/// </summary>
		/// <param name="Parameters">The passed Parameters for text creation</param>
		/// <returns></returns>
		public static MagickReadSettings TextSettingsGeneration(ParamForTextCreation Parameters)
		{
			MagickReadSettings SettingsTextLinear = new MagickReadSettings

			{
				Font = Parameters.Font,
				FillColor = MagickColor.FromRgb(Parameters.FillColor.Red, Parameters.FillColor.Green, Parameters.FillColor.Blue),
				StrokeColor = MagickColor.FromRgb(Parameters.StrokeColor.Red, Parameters.StrokeColor.Green, Parameters.StrokeColor.Blue),
				BorderColor = MagickColor.FromRgb(Parameters.BorderColor.Red, Parameters.BorderColor.Green, Parameters.BorderColor.Blue),
				StrokeWidth = 5,
				// FontPointsize = Parameters.FontPointSize,
				FontWeight = FontWeight.UltraBold,
				BackgroundColor = MagickColors.Transparent,
				Height = Parameters.HeightOfBox, // height of text box
				Width = Parameters.WidthOfBox, // width of text box
			};

			return SettingsTextLinear;
		}

		/// <summary>
		/// Creates our directories for operations
		/// </summary>
		/// <param name="outputDir"></param>
		/// <param name="TextAdded"></param>
		/// <param name="YTDL"></param>
		public static void CreateDirectories(string outputDir, string TextAdded, string YTDL)
		{
			Directory.CreateDirectory(outputDir);
			Directory.CreateDirectory(TextAdded);
			Directory.CreateDirectory(YTDL);
		}

		/// <summary>
		/// Decides if there is a face detected on upper half or lower half of the image
		/// </summary>
		/// <param name="bitmap">the picture to detect faces on</param>
		/// <param name="faceRect">the returned rectangle with the faceposition</param>
		/// <returns></returns>
		public static Point GettextPosition(Bitmap bitmap, Rectangle faceRect)
		{
			List<Point> BoxPositions = new List<Point>();

			int borderBoxTopValueX = 0;
			int borderBoxTopValueY = 0;
			Point borderBoxTop = new Point(borderBoxTopValueX, borderBoxTopValueY);
			BoxPositions.Add(borderBoxTop);

			int borderBoxBottomValueX = 0;
			int borderBoxBottomValueY = bitmap.Height / 2;
			Point BorderBoxBottom = new Point(borderBoxBottomValueX, borderBoxBottomValueY);
			BoxPositions.Add(BorderBoxBottom);

			int topLeftCornerBoxValueX = 0;
			int topLeftCornerBoxValueY = 0;
			Point TopLeftCornerBox = new Point(topLeftCornerBoxValueX, topLeftCornerBoxValueY);
			BoxPositions.Add(TopLeftCornerBox);

			int topRightCornerBoxValueX = bitmap.Width / 2;
			int topRightCornerBoxValueY = 0;
			Point topRightCornerBox = new Point(topRightCornerBoxValueX, topRightCornerBoxValueY);
			BoxPositions.Add(topRightCornerBox);

			int bottomLeftCornerBoxValueX = 0;
			int bottomLeftCornerBoxValueY = bitmap.Height / 2;
			Point bottomLeftCornerBox = new Point(bottomLeftCornerBoxValueX, bottomLeftCornerBoxValueY);
			BoxPositions.Add(bottomLeftCornerBox);

			int bottomRightCornerBoxValueX = bitmap.Width / 2;
			int bottomRightCornerBoxValueY = bitmap.Height / 2;
			Point bottomRightCornerBox = new Point(bottomRightCornerBoxValueX, bottomRightCornerBoxValueY);
			BoxPositions.Add(bottomRightCornerBox);


			const int splitByHalf = 2;
			Point PosOfText;

			int LocationOfRectangleCenterYpos = faceRect.Y + faceRect.Height / splitByHalf;

			// sets the position to the middle of the picture, mid point at X = 0
			int sourceIMGMiddleY = bitmap.Height / splitByHalf; 

			// if middle of image is more then the location of the rectangle height position 
			if (sourceIMGMiddleY > LocationOfRectangleCenterYpos)
			{
				// make text appear on lower half
				// the integer relative position is the height of the image split by 6, which gives a percentage of 
				// the composed image box relative location
				const int splitByPercent = 6;
				int relativePosition = bitmap.Height - (bitmap.Height / splitByPercent);

				PosOfText = new Point(0, relativePosition);
			}
			else
			{
				// make text appear on upper half at the 0,0 initial point 
				PosOfText = new Point(0,0);
			}

			return PosOfText;
		}

		/// <summary>
		/// Produces Picture using the ImageMagick Library
		/// </summary>
		/// <param name="PicData">PictureDataObject Containing everything needed to create an image</param>
		/// <param name="outputFullPath">The Directory where the output images goes</param>
		public static void ProduceTextPictures(PictureData PicData, string outputFullPath)
		{
			using (MagickImage outputImage = new MagickImage(PicData.FileName))
			{
				using (var caption = new MagickImage($"caption:{PicData.ParamForTextCreation.Text}", PicData.ReadSettings))
				{
					// Add the caption layer on top of the background image
					outputImage.Composite(caption, PicData.ParamForTextCreation.PositionOfText.X, PicData.ParamForTextCreation.PositionOfText.Y, CompositeOperator.Over);
					//outputImage.Composite(caption, 0, PicData.ParamForTextCreation.HeightOfBox / 2, CompositeOperator.Over);
					//outputImage.Composite(caption, PicData.ParamForTextCreation.WidthOfBox / 2, PicData.ParamForTextCreation.HeightOfBox, CompositeOperator.Over);
					//outputImage.Composite(caption, PicData.ParamForTextCreation.WidthOfBox / 2, PicData.ParamForTextCreation.HeightOfBox / 2, CompositeOperator.Over);

					outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);

					outputImage.Quality = 100;

					// outputs the file to the provided path and name
					outputImage.Write(outputFullPath);
				}

			}

		}

		/// <summary>
		/// Creates Variety from an existing image, this will be on user interaction
		/// </summary>
		/// <param name="PictureInputData">The Image to create variety of</param>
		/// <param name="TargetFolder">The Folder where you want the results to go</param>
		public static void CreateVariety(PictureData PictureInputData, string TargetFolder)
		{
			const float baseLuminanceValue = 0.50F;
			float fillcolorHue = PictureInputData.ParamForTextCreation.FillColor.Hue;
			float fillcolorLuminance = baseLuminanceValue;

			float strokecolorHue = PictureInputData.ParamForTextCreation.StrokeColor.Hue;
			float strokecolorLuminance = baseLuminanceValue;

			float bordercolorHue = PictureInputData.ParamForTextCreation.BorderColor.Hue;
			float bordercolorLuminance = baseLuminanceValue;

			// create variety based on the current value
			List<float> VarietyList = new List<float>();

			float Variety1 = 0.30F;
			VarietyList.Add(Variety1);

			float Variety2 = 0.45F;
			VarietyList.Add(Variety2);

			float Variety3 = 0.55F;
			VarietyList.Add(Variety3);

			float Variety4 = 0.85F;
			VarietyList.Add(Variety4);

			float Variety5 = 1F;
			VarietyList.Add(Variety5);

			foreach (float variety in VarietyList)
			{
				PictureData VarietyData = new PictureData
				{
					ParamForTextCreation = PictureInputData.ParamForTextCreation
				};

				VarietyData.ParamForTextCreation.FillColor.SetByHSL(fillcolorHue, variety, fillcolorLuminance);

				VarietyData.ParamForTextCreation.StrokeColor.SetByHSL(strokecolorHue, variety, strokecolorLuminance);

				VarietyData.ParamForTextCreation.BorderColor.SetByHSL(bordercolorHue, variety, bordercolorLuminance);

				VarietyData.FileName = PictureInputData.FileName;

				Directory.CreateDirectory(TargetFolder + "//" + "variety of " + Path.GetFileName(VarietyData.FileName));

				string outpath = TargetFolder + "//" + "variety of " + Path.GetFileName(VarietyData.FileName) + $"//{variety}" + ".png";

				MagickReadSettings settings = TextSettingsGeneration(VarietyData.ParamForTextCreation);

				VarietyData.ReadSettings = settings;

				PictureInputData.Varieties.Add(VarietyData);

				ProduceTextPictures(VarietyData, outpath);
			}

		}

		///// <summary>
		///// This isnt working, it throws a major exception for whatever reason
		///// </summary>
		///// <param name="URL"></param>
		///// <returns>Returns the name of the video of the specified URL</returns>
		//public static async Task<string> FetchURLTitleOfVideo(string URL)
		//{
		//	var ytdl = new YoutubeDL
		//	{
		//		// set paths
		//		YoutubeDLPath = "..\\..\\yt-dlp.exe",
		//		FFmpegPath = "YTDL/ffmpeg.exe",
		//	};
		//	var res = await ytdl.RunVideoDataFetch(URL);
		//	// get some video information
		//	VideoData video = res.Data;
		//	string title = video.Title;
		//	string uploader = video.Uploader;
		//	long? views = video.ViewCount;

		//	return title;
		//}

		//public static void AddNewLineToString(string stringToVerticalize)
		//{
		//	char[] arrayedText = stringToVerticalize.ToCharArray();
			
		//	string newLinedText;

		//	foreach(char c in arrayedText)
		//	{
		//		// newLinedText.Insert(c + "\n");
		//	}

		//}

	}

}