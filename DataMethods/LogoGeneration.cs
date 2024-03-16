// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using SkiaSharp;

namespace BulkThumbnailCreator.DataMethods
{
    public static class LogoGeneration
    {
        public static void GenerateLogo(string outputDir)
        {
            // Create a SKImageInfo object for the canvas size
            SKImageInfo info = new(1000, 125);

            // Create a SKSurface to draw on using the SKImageInfo
            using var surface = SKSurface.Create(info);
            // Get the canvas from the surface
            var canvas = surface.Canvas;

            // Create a paint object for drawing text
            using (SKPaint textPaint = new())
            {
                // Set the text size and typeface
                textPaint.TextSize = 72;
                textPaint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

                // Create a random number generator
                Random random = new();

                // Generate random start and end colors
                SKColor startColor = new((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                SKColor endColor = new((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

                // Create a gradient shader
                var gradientShader = SKShader.CreateLinearGradient(
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
            using var skImage = surface.Snapshot();
            using var data = skImage.Encode();
            try
            {
                using var stream = File.OpenWrite(outputDir);
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
