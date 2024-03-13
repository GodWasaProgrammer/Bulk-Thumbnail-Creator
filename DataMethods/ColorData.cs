namespace BulkThumbnailCreator;

public class ColorData
{
    static readonly Random Random = new();
    public static List<string> selectedColors = [];
    public static ParamForTextCreation SelectTwoRandomColors(ParamForTextCreation paramIn)
    {
        MagickColor firstcolor;
        MagickColor secondColor;

        List<string> colorList = GetAllMagickColors();

        colorList.Remove("None");
        colorList.Remove("Transparent");
        colorList.Remove("RebeccaPurple");

        foreach (var alreadyselectedcolor in selectedColors)
        {
            colorList.Remove(alreadyselectedcolor);
        }

        int randomIndexofFirstColor = Random.Next(colorList.Count);
        firstcolor = new(colorList[randomIndexofFirstColor]);
        selectedColors.Add(colorList[randomIndexofFirstColor]);
        paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);

        int randomIndexofSecondColor = Random.Next(colorList.Count);
        secondColor = new(colorList[randomIndexofSecondColor]);
        selectedColors.Add(colorList[randomIndexofSecondColor]);
        paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);

        return paramIn;
    }

    public static ParamForTextCreation SelectTwoDifferentColors(ParamForTextCreation paramIn)
    {
        MagickColor firstcolor;
        MagickColor secondColor;

        List<string> colorList = GetAllMagickColors();

        colorList.Remove("None");
        colorList.Remove("Transparent");
        colorList.Remove("RebeccaPurple");

        foreach (var alreadyselectedcolor in selectedColors)
        {
            colorList.Remove(alreadyselectedcolor);
        }

        for (int i = 0; i < colorList.Count; i++)
        {
            if (i == 0)
            {
                firstcolor = new(colorList[i]);
                selectedColors.Add(colorList[i]);
                paramIn.FillColor.SetByRGB((byte)firstcolor.R, (byte)firstcolor.G, (byte)firstcolor.B);
            }

            if (i == colorList.Count - 1)
            {
                string PassColor = colorList[i - 1];
                secondColor = new(PassColor);
                selectedColors.Add(colorList[i]);
                paramIn.StrokeColor.SetByRGB((byte)secondColor.R, (byte)secondColor.G, (byte)secondColor.B);
            }
        }
        return paramIn;
    }

    private static List<string> GetAllMagickColors()
    {
        List<string> colorsList = [];

        Type magickColorsType = typeof(MagickColors);
        PropertyInfo[] properties = magickColorsType.GetProperties(BindingFlags.Public | BindingFlags.Static);

        foreach (PropertyInfo property in properties)
        {
            if (property.PropertyType == typeof(MagickColor))
            {
                colorsList.Add(property.Name);
            }
        }
        return colorsList;
    }
}