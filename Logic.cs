﻿using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

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
		/// <returns>returns a MagickColor Object which is basically RGB</returns>
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

		static readonly XmlSerializer serializer = new XmlSerializer(typeof(List<string>));

		public static void DeSerializeDownloadedVideosList()
		{
			if (File.Exists(BTCSettings.PathToXMLListOfDownloadedVideos))
			{
				using (FileStream file = File.OpenRead(BTCSettings.PathToXMLListOfDownloadedVideos))
				{
					BTCSettings.DownloadedVideosList = (List<string>)serializer.Deserialize(file);
				}

			}
		}

		public static void SerializeDownloadedVideosList()
		{
			using (FileStream file = File.Create(BTCSettings.PathToXMLListOfDownloadedVideos))
			{
				serializer.Serialize(file, BTCSettings.DownloadedVideosList);
			}
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
		/// <param name="scheme"></param>
		/// <returns>Returns the generated MagickReadSettings</returns>
		public static MagickReadSettings Linear(ParamForTextCreation scheme)
		{
			
			MagickReadSettings SettingsTextLinear = new MagickReadSettings
			{
				Font = "linear",
				FillColor = MagickColor.FromRgb(scheme.FillColor.Red, scheme.FillColor.Green, scheme.FillColor.Blue),
				StrokeColor = MagickColor.FromRgb(scheme.StrokeColor.Red, scheme.StrokeColor.Green, scheme.StrokeColor.Blue),
				BorderColor = MagickColor.FromRgb(scheme.BorderColor.Red, scheme.BorderColor.Green, scheme.BorderColor.Blue),
				FontStyle = FontStyleType.Bold,
				StrokeAntiAlias = true,
				StrokeWidth = 6,
				FontPointsize = scheme.FontPointSize,
				FontWeight = FontWeight.UltraBold,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				// Width = 1700, // width of text box
			};

			return SettingsTextLinear;
		}

	}

}