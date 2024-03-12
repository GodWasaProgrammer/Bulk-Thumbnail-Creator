using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace BulkThumbnailCreator;

public class ColorData
{
    /// Generates random colors in bytes
    /// </summary>
    /// <returns>returns a MagickColor Object which is RGB</returns>
    internal static MagickColor RandomizeColor()
    {
        Random colorRandom = new();
        byte pickedColorRedRGB = (byte)colorRandom.Next(Settings.MaxRGB);
        byte pickedColorGreenRGB = (byte)colorRandom.Next(Settings.MaxRGB);
        byte pickedColorBlueRGB = (byte)colorRandom.Next(Settings.MaxRGB);

        MagickColor colorRNGPicked;

        colorRNGPicked = MagickColor.FromRgb(pickedColorRedRGB, pickedColorGreenRGB, pickedColorBlueRGB);

        return colorRNGPicked;
    }

    /// <summary>
    /// Allows you to "spin" the HSL "globe" to "invert" colors
    /// </summary>
    /// <param name="inputHue">The Hue input Value to invert</param>
    /// <returns>the inverted hue value spun 180 degrees(float)</returns>
    public static float ColorWheelSpinner(float inputHue)
    {
        float fullSpin = 180F;

        if (inputHue < 180F)
        {
            inputHue += fullSpin;
        }
        else
        {
            inputHue -= fullSpin;
        }

        return inputHue;
    }

    /// <summary>
    /// Generates Color output to be used in a PictureData Object to generate text colors
    /// </summary>
    /// <param name="InputParameter">ParamForTextcreation Object to Generate Colors for</param>
    /// <param name="currentelement">the current index of the object being passed</param>
    /// <returns>returns the ParamForTextCreation object with the modified Color Values</returns>
    public static ParamForTextCreation DecideColorGeneration(ParamForTextCreation InputParameter)
    {
        float hueFillColor = 0F;
        float saturationFillColor = 1F;
        float lightnessFillColor = 0.50F;
        float saturationStrokeColor = 1F;
        float lightnessStrokeColor = 0.50F;

        const float maxHueValue = 360F;
        const float incrementalColor = 12.5F;
        const float resetFromMaxToMin = 0F;

        hueFillColor += incrementalColor;

        float hueStrokeColor = ColorData.ColorWheelSpinner(hueFillColor);

        if (hueFillColor > maxHueValue)
        {
            hueFillColor = resetFromMaxToMin;
        }

        InputParameter.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);

        InputParameter.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);

        return InputParameter;
    }

    /// <summary>
    /// Generates randomized colorvalues 
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static ParamForTextCreation GenerateRandomColorSettings(ParamForTextCreation param)
    {
        MagickColor magickColor = RandomizeColor();
        param.FillColor.SetByRGB((byte)magickColor.R, (byte)magickColor.G, (byte)magickColor.B);

        magickColor = ColorData.RandomizeColor();
        param.StrokeColor.SetByRGB((byte)magickColor.R, (byte)magickColor.G, (byte)magickColor.B);

        return param;
    }

    static Random hueRandom = new Random(396235);
    static Random satRandom = new Random(1112434);
    static Random LightRandom = new Random(35325323);
    /// <summary>
    /// Generates Color output to be used in a PictureData Object to generate text colors
    /// </summary>
    /// <param name="InputParameter">ParamForTextcreation Object to Generate Colors for</param>
    /// <param name="currentelement">the current index of the object being passed</param>
    /// <returns>returns the ParamForTextCreation object with the modified Color Values</returns>
    public static ParamForTextCreation DecideColorGenerationAlt(ParamForTextCreation InputParameter)
    {
        float hueFill;
        float hueStroke;

        bool fillOkay = false;
        bool strokeOkay = false;
        int MaxHue = 360;
        do
        {
            hueFill = hueRandom.Next(MaxHue);
            hueStroke = hueRandom.Next(MaxHue);

            // Check if the difference between the new value and any existing value is within the range

            if (Math.Abs(hueFill - hueStroke) >= 35)
            {
                fillOkay = true;
                strokeOkay = true;
            }

        } while (!fillOkay || !strokeOkay);

        float satFill;
        float satStroke;

        bool satFillColorOkay = false;
        bool satStrokeColorOkay = false;

        do
        {
            satFill = (float)satRandom.NextDouble() + 0.35F;
            satStroke = (float)satRandom.NextDouble() + 0.35F;

            if (Math.Abs(satFill - satStroke) >= 0.25)
            {
                satFillColorOkay = true;
                satStrokeColorOkay = true;
            }

        } while (!satFillColorOkay || !satStrokeColorOkay);

        float lightness;
        do
        {
            lightness = (float)LightRandom.NextDouble();
        } while (lightness < 0.25 || lightness > 0.60);

        InputParameter.FillColor.SetByHSL(hueFill, satFill, lightness);

        InputParameter.StrokeColor.SetByHSL(hueStroke, satStroke, lightness);

        return InputParameter;
    }

    static Random Random = new Random();
    public static List<string> selectedColors = new List<string>();
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
        List<string> colorsList = new List<string>();

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