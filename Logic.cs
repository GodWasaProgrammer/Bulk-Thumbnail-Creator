using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
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
		/// <param name="sourcePicture">the picture to detect faces on</param>
		/// <param name="faceRect">the returned rectangle with the faceposition</param>
		/// <returns></returns>
		public static ParamForTextCreation GettextPosition(ParamForTextCreation parameters, Bitmap sourcePicture, Rectangle faceRect)
		{
			Dictionary<Box, Rectangle> Boxes = new Dictionary<Box, Rectangle>();
			
			// top box
			int borderBoxTopValueX = 0;
			int borderBoxTopValueY = 0;
			Box topBox = Box.TopBox;

			Rectangle borderBoxTopRectangle = new Rectangle
			{
				X = borderBoxTopValueX,
				Y = borderBoxTopValueY,
				Width = sourcePicture.Width,
				Height = borderBoxTopValueY
			};

			Boxes.Add(topBox, borderBoxTopRectangle);

			// bottom box
			int bottomBoxValueX = 0;
			int bottomBoxValueY = sourcePicture.Height / 2;
			Box bottomBox = Box.BottomBox;

			Rectangle bottomBoxRectangle = new Rectangle
			{
				X = bottomBoxValueX,
				Y = bottomBoxValueY,
				Width = sourcePicture.Width,
				Height = bottomBoxValueY
			};

			Boxes.Add(bottomBox, bottomBoxRectangle);

			// top left box
			int topLeftBoxValueX = 0;
			int topLeftBoxValueY = 0;
			Box topLeftBox = Box.TopLeft;

			Rectangle topLeftBoxRectangle = new Rectangle
			{
				X = topLeftBoxValueX,
				Y = topLeftBoxValueY,
				Width = sourcePicture.Width / 2,
				Height = topLeftBoxValueY / 2
			};

			Boxes.Add(topLeftBox, topLeftBoxRectangle);

			// top right box
			int topRightBoxValueX = sourcePicture.Width / 2;
			int topRightBoxValueY = 0;
			Box topRightBox = Box.TopRight;

			Rectangle topRightBoxRectangle = new Rectangle
			{
				X = topRightBoxValueX,
				Y = topRightBoxValueY,
				Width = sourcePicture.Width / 2,
				Height = sourcePicture.Height / 2
			};

			Boxes.Add(topRightBox, topRightBoxRectangle);

			// bottom left box
			int bottomLeftBoxValueX = 0;
			int bottomLeftBoxValueY = sourcePicture.Height / 2;
			Box bottomLeftBox = Box.BottomLeft;

			Rectangle bottomleftBoxRectangle = new Rectangle
			{
				X = bottomLeftBoxValueX,
				Y = bottomLeftBoxValueY,
				Width = sourcePicture.Width / 2,
				Height = sourcePicture.Height / 2
			};

			Boxes.Add(bottomLeftBox, bottomleftBoxRectangle);

			// bottom right box
			int bottomRightBoxValueX = sourcePicture.Width / 2;
			int bottomRightBoxValueY = sourcePicture.Height / 2;
			Box bottomRightBox = Box.BottomRight;

			Rectangle bottomRightBoxRectangle = new Rectangle
			{
				X = bottomRightBoxValueX,
				Y = bottomRightBoxValueY,
				Width = sourcePicture.Width / 2,
				Height = sourcePicture.Height / 2
			};

			Boxes.Add(bottomRightBox,bottomRightBoxRectangle);
			
			List<Box> BoxesWithNoFacesDetected = new List<Box>();

			// calculate position of face rectangle in relation to boxes

			//bool topleftbox = faceRect.X < sourcePicture.Width / 2 && faceRect.Y < sourcePicture.Height / 2;

			//if (!topleftbox)
			//{
			//	BoxesWithNoFacesDetected.Add(topLeftBox);
			//}

			// top left cornerbox no face detected
			if (!(faceRect.X < sourcePicture.Width / 2 && faceRect.Y < sourcePicture.Height / 2))
			{
				BoxesWithNoFacesDetected.Add(topLeftBox);
			}
	

			bool toprightbox = faceRect.X < sourcePicture.Width / 2 && faceRect.Y > sourcePicture.Height / 2;

			if (!toprightbox)
			{
				BoxesWithNoFacesDetected.Add(topRightBox);
			}

			//// top right corner box
			//if (faceRect.X > sourcePicture.Width / 2 && faceRect.Y < sourcePicture.Height / 2)
			//{
			//	// face is detected in top right corner box
			//	// disallow box
			//}
			//else
			//{
			//	BoxesWithNoFacesDetected.Add(topRightBox);
			//}

			bool bottomleftbox = faceRect.X < sourcePicture.Width / 2 && faceRect.Y <= sourcePicture.Height / 2;
			if (!bottomleftbox)
			{
				BoxesWithNoFacesDetected.Add(bottomLeftBox);
			}

			//// bottom left corner box
			//if (faceRect.X > sourcePicture.Width / 2 && faceRect.Y > sourcePicture.Height/ 2)
			//{
			//	// face is detected in bottom left corner box
			//	// disallow box
			//}
			//else
			//{
			//	BoxesWithNoFacesDetected.Add(bottomLeftBox);
			//}

			bool bottomrightbox = faceRect.X < sourcePicture.Width / 2 && faceRect.Y > sourcePicture.Height / 2;

			if (!bottomrightbox)
			{
				BoxesWithNoFacesDetected.Add(bottomRightBox);
			}
			//// bottom right corner box
			//if (faceRect.X > sourcePicture.Width / 2 && faceRect.Y > sourcePicture.Height / 2)
			//{
			//	// face is detected in bottom right corner box
			//	// disallow box

			//}
			//else
			//{
			//	BoxesWithNoFacesDetected.Add(bottomRightBox);
			//}

			// handles the calculation of faces if the boxes are top/bottom boxes
			int LocationOfRectangleCenterYpos = faceRect.Y + faceRect.Height / 2;

			// sets the position to the middle of the picture, mid point at X = 0
			int sourceIMGMiddleY = sourcePicture.Height / 2;

			// if middle of image is more then the location of the rectangle height position 
			if (sourceIMGMiddleY > LocationOfRectangleCenterYpos)
			{
				// add bottom box to list of allowed boxes
				BoxesWithNoFacesDetected.Add(bottomBox);

			}
			
			// if middle of image is less then the location of the rectangle height position
			if (sourceIMGMiddleY < LocationOfRectangleCenterYpos)
			{
				BoxesWithNoFacesDetected.Add(topBox);
			}

			// write boxvalues and name to object
			parameters.Boxes = Boxes;

			// all calculations done, pick one box
			Random randomizeBoxPosition = new Random();
			int randomizedbox = randomizeBoxPosition.Next(BoxesWithNoFacesDetected.Count);

			// picks a random box that has no face in it
			Box pickedBoxName = BoxesWithNoFacesDetected[randomizedbox];
			
			// tries to read from dictionary
			Boxes.TryGetValue(pickedBoxName, out Rectangle pickedBoxRectangle);

			parameters.CurrentBox = pickedBoxName;

			// makes a point to feed to the parameters passed in
			Point pickedBoxPoint = new Point(pickedBoxRectangle.X, pickedBoxRectangle.Y);
			
			// sets the position of the parameter objects point variable 
			parameters.PositionOfText = pickedBoxPoint;

			// passes the box width and height to the parameter passed to the method
			// this is important to avoid boxes being juxtaposed outside of the image
			parameters = CalculateBoxData(pickedBoxName, sourcePicture, parameters);

			//if (pickedBoxName == "TopBox" || pickedBoxName == "BottomBox")
			//{
			//	parameters.WidthOfBox = sourcePicture.Width;
			//	parameters.HeightOfBox = sourcePicture.Height / 2;
			//}
			//else
			//{
			//	parameters.WidthOfBox = sourcePicture.Width / 2;
			//	parameters.HeightOfBox = sourcePicture.Height / 2;
			//}

			return parameters;
		}

		public static void ProduceFontVarietyOnClick(PictureData PicToVarietize, string TargetFolder)
		{
			List<string> fontList = new List<string>();

			fontList.Add(PicToVarietize.ParamForTextCreation.Font);

			int FontsToPick = 5;

			for (int i = 0; i < FontsToPick; i++)
			{
				string pickedFont = PickRandomFont();

				// if the list doesnt contain this font already, add it.
				if (!fontList.Contains(pickedFont))
				{
					fontList.Add(pickedFont);
				}
				else
				{
					i--;
				}
				
			}
			// variety selection finished, proceed to creating

			// create the actual varieties
			int counter = 0;
			foreach (string font in fontList)
			{
				counter++;
				PictureData createFontVariety = new PictureData();
				createFontVariety = PicToVarietize;
				createFontVariety.ParamForTextCreation.Font = font;

				Directory.CreateDirectory(TargetFolder + "//" + "variety of " + Path.GetFileName(createFontVariety.FileName));
				
				// still to fix, the pathing turns out incorrect because Font returns Font//nameoffont  

				string outpath = TargetFolder + "//" + "variety of " + Path.GetFileName(createFontVariety.FileName) + $"//variety of:{createFontVariety.FileName} + {Path.GetFileNameWithoutExtension(createFontVariety.ParamForTextCreation.Font)}" + $"{counter}" + ".png";

				ProduceTextPictures(createFontVariety, outpath);
			}

		}

		public static ParamForTextCreation CalculateBoxData(Box CurrentBox, Bitmap sourcePicture , ParamForTextCreation CurrentParamForText)
		{
			if (CurrentBox == Box.TopBox || CurrentBox == Box.BottomBox)
			{
				CurrentParamForText.WidthOfBox = sourcePicture.Width;
				CurrentParamForText.HeightOfBox = sourcePicture.Height / 2;
			}
			else
			{
				CurrentParamForText.WidthOfBox = sourcePicture.Width / 2;
				CurrentParamForText.HeightOfBox = sourcePicture.Height / 2;
			}

			return CurrentParamForText;
		}

		public static void ProducePlacementOfTextVarietyOnClick(PictureData PicToVarietize, string TargetFolder)
		{
			Box boxToExclude = PicToVarietize.ParamForTextCreation.CurrentBox;

			foreach (var CurrentBox in PicToVarietize.ParamForTextCreation.Boxes)
			{
				if (CurrentBox.Key != boxToExclude)
				{

					// lift triangle
					Rectangle currentRectangle = CurrentBox.Value;
					
					// write it to a pint
					Point CurrentPoint = new Point(currentRectangle.X,currentRectangle.Y);
					
					// feed it back into object
					PicToVarietize.ParamForTextCreation.PositionOfText = CurrentPoint;
					Bitmap sourcePicture = new Bitmap(PicToVarietize.FileName);

					// set the currentbox to the currentbox key
					PicToVarietize.ParamForTextCreation.CurrentBox = CurrentBox.Key;

					// calculate box data, important for TextSettingsGeneration returning a correct ReadSettings object
					PicToVarietize.ParamForTextCreation = CalculateBoxData(CurrentBox.Key, sourcePicture, PicToVarietize.ParamForTextCreation);

					PicToVarietize.ReadSettings = TextSettingsGeneration(PicToVarietize.ParamForTextCreation);

					// add it to list of objects created varieties
					PicToVarietize.Varieties.Add(PicToVarietize);

					Directory.CreateDirectory(TargetFolder + "//" + "variety of " + Path.GetFileName(PicToVarietize.FileName));

					string outpath = TargetFolder + "//" + "variety of " + Path.GetFileName(PicToVarietize.FileName) + $"//{CurrentBox.Key}" + ".png";

					ProduceTextPictures(PicToVarietize, outpath);
				}

			}

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