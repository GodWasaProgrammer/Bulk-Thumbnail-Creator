namespace BulkThumbnailCreator;

public class ColorData
{
    /// <summary>
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
        float saturationBorderColor = 1F;
        float lightnessBorderColor = 0.50F;

        const float maxHueValue = 360F;
        const float incrementalColor = 12.5F;
        const float resetFromMaxToMin = 0F;

        hueFillColor += incrementalColor;

        float hueStrokeColor = ColorData.ColorWheelSpinner(hueFillColor);
        float hueBorderColor = ColorData.ColorWheelSpinner(hueFillColor);

        if (hueFillColor > maxHueValue)
        {
            hueFillColor = resetFromMaxToMin;
        }

        InputParameter.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);

        InputParameter.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);

        InputParameter.BorderColor.SetByHSL(hueBorderColor, saturationBorderColor, lightnessBorderColor);

        return InputParameter;
    }

    /// <summary>
    /// Generates randomized colorvalues 
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static ParamForTextCreation GenerateRandomColorSettings(ParamForTextCreation param)
    {
        MagickColor magickColor = ColorData.RandomizeColor();
        param.FillColor.SetByRGB((byte)magickColor.R, (byte)magickColor.G, (byte)magickColor.B);

        magickColor = ColorData.RandomizeColor();
        param.StrokeColor.SetByRGB((byte)magickColor.R, (byte)magickColor.G, (byte)magickColor.B);

        magickColor = ColorData.RandomizeColor();
        param.BorderColor.SetByRGB((byte)magickColor.R, (byte)magickColor.G, (byte)magickColor.B);

        return param;
    }
}
