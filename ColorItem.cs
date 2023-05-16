using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMapx.Colorspace;

namespace Bulk_Thumbnail_Creator
{
	public class ColorItem
	{
		private byte red;
		public byte Red { get { return red; } }

		private byte green;
		public byte Green { get { return green; } }

		private byte blue;
		public byte Blue { get { return blue; } }

		private int hue;
		private float saturation;
		private float luminance;


		// should take HSL and set the correlating RGB values of the object
		void ColorToRGB(HSL inputHSL)
		{
			RGB colorInRGB = inputHSL.ToRGB;

			red = colorInRGB.Red;
			green = colorInRGB.Green;
			blue = colorInRGB.Blue;
		}

		// should take RGB and set the correlating HSL values of the object
		void ColorToHSL(RGB inputRGB)
		{
			HSL colorInHSL = HSL.FromRGB(inputRGB);

			colorInHSL.Hue = hue;
			colorInHSL.Saturation = saturation;
			colorInHSL.Lightness = luminance;
		}

		// set color object by RGB input
		public RGB SetByRGB(byte inputRed, byte inputGreen, byte inputBlue)
		{
			red = inputRed;
			green = inputGreen;
			blue = inputBlue;

			RGB outputRGB = new RGB(red, green, blue);

			ColorToHSL(outputRGB);
			return outputRGB;
		}

		// set color by HSL input
		public HSL SetByHSL(int inputHue, float inputSaturation, float inputLuminance)
		{
			hue = inputHue;
			saturation = inputSaturation;
			luminance = inputLuminance;

			HSL outputHSL = new HSL(hue, saturation, luminance);

			ColorToRGB(outputHSL);
			return outputHSL;
		}

	}

}
