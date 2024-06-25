namespace BulkThumbnailCreator;

public static class ColorData
{
    static readonly Random s_random = new();
    const int MAX_COLORS = 140;

    public static List<string> SelectedColors { get; set; } = [];

    public static ParamForTextCreation SelectTwoRandomColors(ParamForTextCreation paramIn)
    {
        MagickColor firstcolor;
        MagickColor secondColor;

        var colorList = GetAllMagickColors();

        if (SelectedColors.Count == 140 || SelectedColors.Count > 140)
        {
            SelectedColors.Clear();
        }

        firstcolor = new(colorList[s_random.Next(colorList.Count)]);
        SelectedColors.Add(firstcolor.ToString());
        paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);

        secondColor = new(colorList[s_random.Next(colorList.Count)]);
        SelectedColors.Add(secondColor.ToString());
        paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);

        return paramIn;
    }

    public static ParamForTextCreation SelectTwoDifferentColors(ParamForTextCreation paramIn)
    {
        MagickColor firstcolor;
        MagickColor secondColor;

        var colorList = GetAllMagickColors();

        if (SelectedColors.Count == 140 || SelectedColors.Count > 140)
        {
            SelectedColors.Clear();
        }

        firstcolor = new(colorList[0]);
        SelectedColors.Add(firstcolor.ToString());
        paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);

        secondColor = new(colorList[^1]);
        SelectedColors.Add(secondColor.ToString());
        paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);

        return paramIn;
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
