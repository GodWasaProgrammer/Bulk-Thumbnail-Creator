using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;

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

		static float hueFillColor = 55F;
		static readonly float saturationFillColor = 1F;
		static readonly float lightnessFillColor = 0.50F;

		static float hueStrokeColor = 125F;
		static readonly float saturationStrokeColor = 1F;
		static readonly float lightnessStrokeColor = 0.50F;

		static float hueBorderColor = 28F;
		static readonly float saturationBorderColor = 1F;
		static readonly float lightnessBorderColor = 0.50F;

		public static ParamForTextCreation DecideColorGeneration(ParamForTextCreation InputParameter, int currentelement)
		{
			if (currentelement > 1)
			{
				hueFillColor += +12.5F;
				hueStrokeColor += +12.5F;
				hueBorderColor += 12.5F;
				
				if (hueFillColor > 360)
				{
					hueFillColor = 0F;
				}

				if (hueStrokeColor > 360)
				{
					hueStrokeColor = 0F;
				}

				if (hueBorderColor > 360)
				{
					hueBorderColor = 0F;
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
			// this is 8 because it achieves a good ratio of text to the height of the picture
			int NumberToSplitBy = 8;

			int FontSize = Height / NumberToSplitBy;

			return FontSize;
		}

		static readonly XmlSerializer serializer = new XmlSerializer(typeof(List<string>));

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

		public static void SerializeListOfStringsToXML(string PathToXML, List<string> ListOfStringsToSerialize)
		{
			using (FileStream file = File.Create(PathToXML))
			{
				serializer.Serialize(file, ListOfStringsToSerialize);
			}

		}

		///// <summary>
		///// This isnt working, it throws a major exception for whatever reason
		///// </summary>
		///// <param name="URL"></param>
		///// <returns>Returns the name of the video of the specified URL</returns>
		public static async Task<string> FetchURLTitleOfVideo(string URL)
		{
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "YTDL/ffmpeg.exe",
			};
			var res = await ytdl.RunVideoDataFetch(URL);
			// get some video information
			VideoData video = res.Data;
			string title = video.Title;
			string uploader = video.Uploader;
			long? views = video.ViewCount;

			return title;
		}

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

		public static string PickRandomFont()
		{

			var fontNames = Directory.GetFiles("Fonts", "*.TTF*");

			Random randompicker = new Random();

			int randommax = fontNames.Length;

			int fontChosen = randompicker.Next(randommax);

			return fontNames[fontChosen].ToString();
		}

		public static MagickReadSettings TextSettingsGeneration(ParamForTextCreation Parameters)
		{
			MagickReadSettings SettingsTextLinear = new MagickReadSettings

			{
				Font = Parameters.Font,
				FillColor = MagickColor.FromRgb(Parameters.FillColor.Red, Parameters.FillColor.Green, Parameters.FillColor.Blue),
				StrokeColor = MagickColor.FromRgb(Parameters.StrokeColor.Red, Parameters.StrokeColor.Green, Parameters.StrokeColor.Blue),
				BorderColor = MagickColor.FromRgb(Parameters.BorderColor.Red, Parameters.BorderColor.Green, Parameters.BorderColor.Blue),
				FontStyle = FontStyleType.Normal,
				StrokeAntiAlias = true,
				StrokeWidth = 3,
				FontPointsize = Parameters.FontPointSize,
				FontWeight = FontWeight.ExtraBold,
				BackgroundColor = MagickColors.Transparent,
				Height = Parameters.FontPointSize + 50, // height of text box
				Width = Parameters.WidthOfBox, // width of text box
			};

			return SettingsTextLinear;
		}

		public static void CreateDirectories(string outputDir, string TextAdded, string YTDL)
		{
			Directory.CreateDirectory(outputDir);
			Directory.CreateDirectory(TextAdded);
			Directory.CreateDirectory(YTDL);
		}

		public static Point GettextPosition(Bitmap bitmap, Rectangle faceRect)
		{
			Point PosOfText;
			int LocationOfRectangleCenterYpos = faceRect.Y + faceRect.Height / 2;

			// sets the position to the middle of the picture, mid point at X = 0
			int sourceIMGMiddleY = bitmap.Height / 2; 

			// if middle of image is more then the location of the rectangle height position 
			if (sourceIMGMiddleY > LocationOfRectangleCenterYpos)
			{
				// make text appear on lower half
				// the integer relative position is the height of the image split by 6, which gives a percentage of 
				// 
				int relativePosition = bitmap.Height - bitmap.Height / 6;

				PosOfText = new Point(0, relativePosition);
			}
			else
			{
				// make text appear on upper half at the 0,0 initial point 
				PosOfText = new Point(0,0);
			}

			return PosOfText;
		}

		public static void ProduceTextPictures(PictureData PicData, string outputFullPath)
		{
			using (MagickImage outputImage = new MagickImage(PicData.FileName))
			{
				using (var caption = new MagickImage($"caption:{PicData.ParamForTextCreation.Text}", PicData.ReadSettings))
				{
					// Add the caption layer on top of the background image
					outputImage.Composite(caption, PicData.ParamForTextCreation.PositionOfText.X, PicData.ParamForTextCreation.PositionOfText.Y, CompositeOperator.Over);

					outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);

					outputImage.Quality = 100;

					// outputs the file to the provided path and name
					outputImage.Write(outputFullPath);
				}

			}

		}

		public static void CreateVariety(PictureData PictureInputData, string TargetFolder)
		{
			float fillcolorHue = PictureInputData.ParamForTextCreation.FillColor.Hue;
			// float fillcolorSaturation = PictureInputData.ParamForTextCreation.FillColor.Saturation;
			float fillcolorLuminance = 0.50F;

			float strokecolorHue = PictureInputData.ParamForTextCreation.StrokeColor.Hue;
			// float strokecolorSaturation = PictureInputData.ParamForTextCreation.StrokeColor.Saturation;
			float strokecolorLuminance = 0.50F;

			float bordercolorHue = PictureInputData.ParamForTextCreation.BorderColor.Hue;
			// float bordercolorSaturation = PictureInputData.ParamForTextCreation.BorderColor.Saturation;
			float bordercolorLuminance = 0.50F;

			// create variety based on the current value
			// default output value is 0.25F

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

	}

}