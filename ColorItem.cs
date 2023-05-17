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

		private float hue;
		public float Hue {get { return hue; } }

		private float saturation;
		public float Saturation { get { return saturation; } }

		private float luminance;
		public float Luminance { get { return luminance; } }


		/// <summary>
		/// takes HSL as input and set the correlating RGB values of the object
		/// </summary>
		/// <param name="inputHSL">Your HSL object input</param>
		private void ColorToRGB(HSL inputHSL)
		{
			RGB colorInRGB;
			colorInRGB = inputHSL.ToRGB;

			red = colorInRGB.Red;
			green = colorInRGB.Green;
			blue = colorInRGB.Blue;

		}

		/// <summary>
		/// should take RGB and set the correlating HSL values of the object
		/// </summary>
		/// <param name="inputRGB">Your RGB object input</param>
		private void ColorToHSL(RGB inputRGB)
		{
			HSL colorInHSL = HSL.FromRGB(inputRGB);

			colorInHSL.Hue = hue;
			colorInHSL.Saturation = saturation;
			colorInHSL.Lightness = luminance;
		}

		/// <summary>
		/// set color object by RGB input, also sets correlating HSL values on object on the fly
		/// </summary>
		/// <param name="inputRed"></param>
		/// <param name="inputGreen"></param>
		/// <param name="inputBlue"></param>
		/// <returns>returns a RGB object</returns>
		public RGB SetByRGB(byte inputRed, byte inputGreen, byte inputBlue)
		{
			red = inputRed;
			green = inputGreen;
			blue = inputBlue;

			RGB outputRGB = new RGB(red, green, blue);

			ColorToHSL(outputRGB);
			return outputRGB;
		}

		/// <summary>
		/// set color object by HSL input, also sets correlating RGB values of the object via ColorToRGB
		/// </summary>
		/// <param name="inputHue"></param>
		/// <param name="inputSaturation"></param>
		/// <param name="inputLuminance"></param>
		/// <returns>returns a HSL object</returns>
		public HSL SetByHSL(float inputHue, float inputSaturation, float inputLuminance)
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
