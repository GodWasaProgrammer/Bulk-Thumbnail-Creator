namespace BulkThumbnailCreator;

public static class ColorData
{
    static readonly Random s_random = new();
    const int MAX_COLORS = 140;

    public static List<string> SelectedColors { get; set; } = [];

    public static void SelectTwoRandomColors(ParamForTextCreation paramIn)
    {
        var colorList = GetAllMagickColors();

        MagickColor firstcolor = new(colorList[s_random.Next(colorList.Count)]);
        SelectedColors.Add(firstcolor.ToString());
        paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);

        colorList = GetAllMagickColors();
        MagickColor secondColor = new(colorList[s_random.Next(colorList.Count)]);
        SelectedColors.Add(secondColor.ToString());
        paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);
    }

    public static void SelectTwoDifferentColors(ParamForTextCreation paramIn)
    {
        var colorList = GetAllMagickColors();

        MagickColor firstcolor = new(colorList[0]);
        SelectedColors.Add(firstcolor.ToString());
        paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);

        MagickColor secondColor = new(colorList[^1]);
        SelectedColors.Add(secondColor.ToString());
        paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);
    }

    private static List<string> GetAllMagickColors()
    {
        List<string> colorsList = [];
        var magickColorsType = typeof(MagickColors);
        var properties = magickColorsType.GetProperties(BindingFlags.Public | BindingFlags.Static);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(MagickColor))
            {
                colorsList.Add(property.Name);
            }
        }
        colorsList.Remove("None");
        colorsList.Remove("Transparent");
        colorsList.Remove("RebeccaPurple");

        if (SelectedColors.Count is MAX_COLORS or > MAX_COLORS)
        {
            SelectedColors.Clear();
        }

        foreach (var alreadyselectedcolor in SelectedColors)
        {
            colorsList.Remove(alreadyselectedcolor);
        }

        return colorsList;
    }

    public static MagickColor MakeQuantumColor(ColorItem colorItem)
    {
        var redFill = (ushort)(colorItem.Red * 65535 / 255);
        var greenFill = (ushort)(colorItem.Green * 65535 / 255);
        var blueFill = (ushort)(colorItem.Blue * 65535 / 255);

        return new MagickColor(redFill, greenFill, blueFill);
    }
}
