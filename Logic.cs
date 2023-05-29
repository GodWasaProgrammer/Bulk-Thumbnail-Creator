using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;

namespace Bulk_Thumbnail_Creator
{
	internal class Logic
	{
		// list of created Presets for printing text to image
		public static List<MagickReadSettings> ListOfSettingsForText { get; set; } = new List<MagickReadSettings>();

		static readonly Random colorRandom = new Random();
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

		static float hueFillColor = 23F;
		static readonly float saturationFillColor = 0.25F;
		static readonly float lightnessFillColor = 0.25F;

		static float hueStrokeColor = 125F;
		static readonly float saturationStrokeColor = 0.25F;
		static readonly float lightnessStrokeColor = 0.25F;

		static float hueBorderColor = 28F;
		static readonly float saturationBorderColor = 0.25F;
		static readonly float lightnessBorderColor = 0.25F;

		public static ParamForTextCreation DecideColorGeneration(ParamForTextCreation InputParameter, int currentelement)
		{
			if (currentelement > 1)
			{
				hueFillColor += +12.5F;
				hueStrokeColor += +12.5F;

				if (currentelement > 5)
				{
					hueBorderColor += 10F;
				}

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
		//public static async Task<string> FetchURLTitleOfVideo(string URL)
		//{
		//	var ytdl = new YoutubeDL
		//	{
		//		// set paths
		//		YoutubeDLPath = "..\\..\\yt-dlp.exe",
		//		FFmpegPath = "YTDL/ffmpeg.exe",
		//		OutputFolder = "YTDL"
		//	};
		//	var res = await ytdl.RunVideoDataFetch(URL);
		//	// get some video information
		//	VideoData video = res.Data;
		//	string title = video.Title;
		//	string uploader = video.Uploader;
		//	long? views = video.ViewCount;

		//	return title;
		//}

		public static async Task<string> YouTubeDL(string URL)
		{
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "YTDL/ffmpeg.exe",
				OutputFolder = "YTDL"
			};
			///
			
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
		/// Creates TextSettings in the form of MagickReadSettings
		/// which includes information retrieved from a schemeobject which in turn uses Coloritems
		/// </summary>
		/// <param name="Parameters"></param>
		/// <returns>Returns the generated MagickReadSettings</returns>
		/// 

		public static string PickRandomFont()
		{
			
			var fontNames = Directory.GetFiles("Fonts", "*.TTF*");
			
			Random randompicker = new Random();

			int randommax = fontNames.Length;

			int fontChosen = randompicker.Next(randommax + 1);

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

		public static void CreateDirectories(string outputDir, string TextAdded,string YTDL)
		{
			Directory.CreateDirectory(outputDir);
			Directory.CreateDirectory(TextAdded);
			Directory.CreateDirectory(YTDL);
		}

		public static Point GettextPosition(Bitmap bitmap, Rectangle faceRect)
		{
			Point PosOfText;
			int LocationOfRectangleCenterYpos = faceRect.Y + faceRect.Height / 2;

			int sourceIMGMiddleY = bitmap.Height / 2;

			if (sourceIMGMiddleY > LocationOfRectangleCenterYpos)
			{
				// make text appear on lower half

				int relativePosition = bitmap.Height - bitmap.Height / 6;

				PosOfText = new Point(0, relativePosition);
			}
			else
			{
				// make text appear on upper half
				PosOfText = new Point(0, sourceIMGMiddleY);
			}

			return PosOfText;
		}

		public static void ProduceTextPictures(int i, Point PosOfText, string outputFullPath, string screenCaptureFile)
		{
			using (MagickImage outputImage = new MagickImage(screenCaptureFile))
			{
				MagickReadSettings settings = Logic.ListOfSettingsForText[i];

				using (var caption = new MagickImage($"caption:{BTCSettings.ListOfText[0]}", settings))
				{
					// Add the caption layer on top of the background image
					outputImage.Composite(caption, PosOfText.X, PosOfText.Y, CompositeOperator.Over);

					outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);

					outputImage.Quality = 100;

					// outputs the file to the provided path and name
					outputImage.Write(outputFullPath);
				}

			}
		}

		public static void CreateVariety(PictureData PictureInputData)
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

			float Variety1 = 0.10F;
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
				PictureInputData.ParamForTextCreation.FillColor.SetByHSL(fillcolorHue, variety, fillcolorLuminance);

				PictureInputData.ParamForTextCreation.StrokeColor.SetByHSL(strokecolorHue,variety,strokecolorLuminance);

				PictureInputData.ParamForTextCreation.BorderColor.SetByHSL(bordercolorHue, variety, bordercolorLuminance);

				// string outpath = $"{Path.GetFullPath(PictureInputData.FileName)}";

				Directory.CreateDirectory(BTCSettings.TextAddedDir + "//" + "variety of " + Path.GetFileName(PictureInputData.FileName));

				string outpath = BTCSettings.TextAddedDir + "//" + "variety of " + Path.GetFileName(PictureInputData.FileName) + $"//{variety}" + ".png";

				// string path = Path.GetFullPath(PictureInputData.FileName);

				// modify magicreadsettings 

				MagickReadSettings settings = TextSettingsGeneration(PictureInputData.ParamForTextCreation);

				int IndexOfFile = PictureInputData.IndexOfFile;

				Logic.ListOfSettingsForText[IndexOfFile] = settings;

				string imageName = Path.GetFileName(PictureInputData.FileName);

				string inputPath = Path.GetFullPath(BTCSettings.OutputDir) + $"/{imageName}";

				ProduceTextPictures(IndexOfFile, PictureInputData.ParamForTextCreation.PositionOfText, outpath, inputPath);
			}


		}

	}

}