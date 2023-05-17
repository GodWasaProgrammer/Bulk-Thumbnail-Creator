using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator
{
	internal class TextScheme
	{
		ColorItem fillColor = new ColorItem();
		public ColorItem FillColor { get { return fillColor; } set { fillColor = FillColor; } }

		ColorItem strokeColor = new ColorItem();
		public ColorItem StrokeColor { get { return strokeColor; } set { strokeColor = StrokeColor; } }

		ColorItem borderColor = new ColorItem();
		public ColorItem BorderColor { get { return borderColor; } set { borderColor = BorderColor; } }

	}


}
