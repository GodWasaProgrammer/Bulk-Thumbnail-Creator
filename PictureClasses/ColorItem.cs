namespace BulkThumbnailCreator.PictureClasses;

public class ColorItem
{
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }

    /// <summary>
    /// Returns you the Saturation of the ColorItem by calling GetHSLValuesFrom RGB and giving you the hue value of coloritem
    /// </summary>
    public float Hue
    {
        get
        {
            var currentHSL = GetHSLValuesFromRGB(Red, Green, Blue);

            return currentHSL.Hue;
        }
    }

    /// <summary>
    /// Returns you the Saturation of the ColorItem by calling GetHSLValuesFrom RGB and giving you the Saturation value of color item
    /// </summary>
    public float Saturation
    {
        get
        {
            var currentHSL = GetHSLValuesFromRGB(Red, Green, Blue);

            return currentHSL.Saturation;
        }
    }

    /// <summary>
    /// Returns you the luminance by calling GETHSLValuesFromRGB and giving you the luminance value of color item
    public float Luminance
    {
        get
        {
            var currentHSL = GetHSLValuesFromRGB(Red, Green, Blue);

            return currentHSL.Lightness;
        }
    }

    /// <summary>
    /// takes HSL as input and set the correlating RGB values of the object
    /// </summary>
    /// <param name="inputHSL">Your HSL object input</param>
    private void ColorToRGB(Hsl inputHSL)
    {
        var colorInRGB = inputHSL.ToRGB();

        Red = colorInRGB.Red;
        Green = colorInRGB.Green;
        Blue = colorInRGB.Blue;
    }

    /// <summary>
    /// set color object by RGB input
    /// </summary>
    /// <param name="inputRed"></param>
    /// <param name="inputGreen"></param>
    /// <param name="inputBlue"></param>
    /// <returns>returns a RGB object</returns>
    public Rgb SetByRGB(byte inputRed, byte inputGreen, byte inputBlue)
    {
        Red = inputRed;
        Green = inputGreen;
        Blue = inputBlue;

        Rgb outputRGB = new(Red, Green, Blue);

        return outputRGB;
    }

    /// <summary>
    /// set color object by HSL input, also sets correlating RGB values of the object via ColorToRGB
    /// </summary>
    /// <param name="inputHue"></param>
    /// <param name="inputSaturation"></param>
    /// <param name="inputLuminance"></param>
    /// <returns>returns a HSL object</returns>
    public Hsl SetByHSL(float inputHue, float inputSaturation, float inputLuminance)
    {
        Hsl outputHSL = new(inputHue, inputSaturation, inputLuminance);

        ColorToRGB(outputHSL);

        return outputHSL;
    }

    /// <summary>
    /// Allows you to get a HSL object back based of the current objects RGB values
    /// </summary>
    /// <param name="inputred">your Red RGB value input</param>
    /// <param name="inputgreen">your Green RGB value input</param>
    /// <param name="inputblue">your Blue RGB value input</param>
    /// <returns></returns>
    private static Hsl GetHSLValuesFromRGB(byte inputred, byte inputgreen, byte inputblue)
    {
        Rgb inputRGB = new(inputred, inputgreen, inputblue)
        {
            Red = inputred,
            Green = inputgreen,
            Blue = inputblue
        };

        var currentHSL = Hsl.FromRGB(inputRGB);

        return currentHSL;
    }

    /// <summary>
    /// Copy Ctor
    /// </summary>
    /// <param name="colorItemToCopy"></param>
    public ColorItem(ColorItem colorItemToCopy)
    {
        Red = colorItemToCopy.Red;
        Green = colorItemToCopy.Green;
        Blue = colorItemToCopy.Blue;
    }

    public ColorItem()
    {
    }
}
