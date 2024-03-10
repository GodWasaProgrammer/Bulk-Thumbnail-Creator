using SkiaSharp;

namespace BulkThumbnailCreator.DataMethods
{
    public class LogoGeneration
    {
        public static void GenerateLogo(string outputDir)
        {
            // Create a SKImageInfo object for the canvas size
            SKImageInfo info = new SKImageInfo(1000, 125);

            // Create a SKSurface to draw on using the SKImageInfo
            using (SKSurface surface = SKSurface.Create(info))
            {
                // Get the canvas from the surface
                SKCanvas canvas = surface.Canvas;

                // Create a paint object for drawing text
                using (SKPaint textPaint = new SKPaint())
                {
                    // Set the text size and typeface
                    textPaint.TextSize = 72;
                    textPaint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

                    // Create a random number generator
                    Random random = new Random();

                    // Generate random start and end colors
                    SKColor startColor = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    SKColor endColor = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

                    // Create a gradient shader
                    SKShader gradientShader = SKShader.CreateLinearGradient(
                        new SKPoint(0, 0),
                        new SKPoint(info.Width, info.Height),
                        new SKColor[] { startColor, endColor },
                        null,
                        SKShaderTileMode.Clamp);

                    // Set the shader for the text paint
                    textPaint.Shader = gradientShader;

                    // Draw the text on the canvas
                    canvas.DrawText("Bulk Thumbnail Creator", 100, 100, textPaint);
                }

                // Encode the surface as a PNG image
                using SKImage skImage = surface.Snapshot();
                using SKData data = skImage.Encode();
                try
                {
                    using FileStream stream = File.OpenWrite(outputDir);
                    data.SaveTo(stream);
                    stream.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
