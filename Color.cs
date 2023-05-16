using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMapx.Colorspace;

namespace Bulk_Thumbnail_Creator
{
	public class Color
	{
		  Color()
		{
			byte red;
			byte green;
			byte blue;

			int hue;
			float saturation;
			float luminance;

			RGB ColorAsRGB()
			{
				RGB colorInRGB;
				colorInRGB = new RGB(red, green, blue);

				HSL colorinHSL = ColorAsHSL();
				colorInRGB = colorinHSL.ToRGB;

				return colorInRGB;
			}

			HSL ColorAsHSL()
			{
				HSL colorInHSL = HSL.FromRGB(ColorAsRGB());

				colorInHSL.Hue = hue;
				colorInHSL.Saturation = saturation;
				colorInHSL.Lightness = luminance;

				return colorInHSL;
			}

		}

	}

}
