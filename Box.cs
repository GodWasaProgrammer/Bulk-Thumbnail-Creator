using Bulk_Thumbnail_Creator.Enums;
using System.Drawing;

namespace Bulk_Thumbnail_Creator
{
    internal class Box
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        BoxType Type { get; set; }

        Rectangle Rectangle { get; set; }

    }

}
