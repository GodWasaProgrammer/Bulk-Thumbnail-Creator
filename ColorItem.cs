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


		/// <summary>
		/// Returns you the Saturation of the ColorItem by calling GetHSLValuesFrom RGB and giving you the hue value of coloritem
		/// </summary>
		public float Hue 
		{
			get 
			{
				HSL CurrentHSL = GetHSLValuesFromRGB(red, green, blue);

				return CurrentHSL.Hue; 
			} 

		}

		/// <summary>
		/// Returns you the Saturation of the ColorItem by calling GetHSLValuesFrom RGB and giving you the Saturation value of coloritem
		/// </summary>
		public float Saturation 
		{ 
			get 
			{
				HSL CurrentHSL = GetHSLValuesFromRGB(red, green, blue);

				return CurrentHSL.Saturation; 
			} 

		}

		/// <summary>
		/// Returns you the luminance by calling GETHSLValuesFromRGB and and giving you the luminance value of coloritem
		/// </summary>
		public float Luminance 
		{ 
			get 
			{
				HSL CurrentHSL = GetHSLValuesFromRGB(red, green, blue);

				return CurrentHSL.Lightness; 
			} 

		}

		/// <summary>
		/// takes HSL as input and set the correlating RGB values of the object
		/// </summary>
		/// <param name="inputHSL">Your HSL object input</param>
		private void ColorToRGB(HSL inputHSL)
		{
			RGB colorInRGB = inputHSL.ToRGB;

			red = colorInRGB.Red;
			green = colorInRGB.Green;
			blue = colorInRGB.Blue;
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
			HSL outputHSL = new HSL(inputHue, inputSaturation, inputLuminance);

			ColorToRGB(outputHSL);

			return outputHSL;
		}

		public static HSL GetHSLValuesFromRGB(byte inputred, byte inputgreen, byte inputblue)
		{
			RGB inputRGB = new RGB
			{
				Red = inputred,
				Green = inputgreen,
				Blue = inputblue
			};

			HSL CurrentHSL = HSL.FromRGB(inputRGB);

			return CurrentHSL;
		}

	}

}
