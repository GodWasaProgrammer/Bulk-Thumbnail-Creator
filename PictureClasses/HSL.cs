// Ignore Spelling: HSL rgb

namespace BulkThumbnailCreator.PictureClasses;

public class Hsl(float hue, float saturation, float lightness)
{
    public float Hue { get; set; } = hue;
    public float Saturation { get; set; } = saturation;
    public float Lightness { get; set; } = lightness;

    public static Hsl FromRGB(Rgb rgb)
    {
        var red = rgb.Red / 255.0f;
        var green = rgb.Green / 255.0f;
        var blue = rgb.Blue / 255.0f;

        var max = Math.Max(red, Math.Max(green, blue));
        var min = Math.Min(red, Math.Min(green, blue));

        var delta = max - min; // Added this line to declare 'delta'

        float hue = 0, saturation, lightness;

        // Calculate hue
        if (max == min)
        {
            hue = 0; // achromatic
        }
        else
        {
            if (max == red)
            {
                hue = ((green - blue) / delta) + (green < blue ? 6 : 0);
            }
            else if (max == green)
            {
                hue = ((blue - red) / delta) + 2;
            }
            else if (max == blue)
            {
                hue = ((red - green) / delta) + 4;
            }
            hue *= 60; // convert to degrees
        }

        // Calculate lightness and saturation
        lightness = (max + min) / 2;

        if (max == min)
        {
            saturation = 0;
        }
        else
        {
            saturation = lightness <= 0.5f ? delta / (max + min) : delta / (2 - max - min);
        }

        return new Hsl(hue, saturation, lightness);
    }

    public Rgb ToRGB()
    {
        var c = (1 - Math.Abs((2 * Lightness) - 1)) * Saturation;
        var x = c * (1 - Math.Abs((Hue / 60 % 2) - 1));
        var m = Lightness - (c / 2);

        float r, g, b;

        if (Hue >= 0 && Hue < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (Hue >= 60 && Hue < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (Hue >= 120 && Hue < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (Hue >= 180 && Hue < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (Hue >= 240 && Hue < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else
        {
            r = c;
            g = 0;
            b = x;
        }

        return new Rgb((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
    }
}
