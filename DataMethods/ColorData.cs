namespace BulkThumbnailCreator;

public static class ColorData
{
    static readonly Random s_random = new();

    public static List<string> SelectedColors { get; set; } = [];

    public static ParamForTextCreation SelectTwoRandomColors(ParamForTextCreation paramIn)
    {
        MagickColor firstcolor;
        MagickColor secondColor;

        var colorList = GetAllMagickColors();

        colorList.Remove("None");
        colorList.Remove("Transparent");
        colorList.Remove("RebeccaPurple");

        if (SelectedColors.Count == 140 || SelectedColors.Count > 140)
        {
            SelectedColors.Clear();
        }
        foreach (var alreadyselectedcolor in SelectedColors)
        {
            colorList.Remove(alreadyselectedcolor);
        }

        firstcolor = new(colorList[s_random.Next(colorList.Count)]);
        SelectedColors.Add(colorList[s_random.Next(colorList.Count)]);
        paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);

        var randomIndexofSecondColor = s_random.Next(colorList.Count);
        secondColor = new(colorList[randomIndexofSecondColor]);
        SelectedColors.Add(colorList[randomIndexofSecondColor]);
        paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);

        return paramIn;
    }

    public static ParamForTextCreation SelectTwoDifferentColors(ParamForTextCreation paramIn)
    {
        MagickColor firstcolor;
        MagickColor secondColor;

        var colorList = GetAllMagickColors();

        colorList.Remove("None");
        colorList.Remove("Transparent");
        colorList.Remove("RebeccaPurple");

        if (SelectedColors.Count == 140 || SelectedColors.Count > 140)
        {
            SelectedColors.Clear();
        }

        foreach (var alreadyselectedcolor in SelectedColors)
        {
            colorList.Remove(alreadyselectedcolor);
        }

        for (var i = 0; i < colorList.Count; i++)
        {
            if (i == 0)
            {
                firstcolor = new(colorList[i]);
                SelectedColors.Add(colorList[i]);
                paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);
            }

            if (i == colorList.Count - 1)
            {
                var passColor = colorList[i - 1];
                secondColor = new(passColor);
                SelectedColors.Add(colorList[i]);
                paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);
            }
        }
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
