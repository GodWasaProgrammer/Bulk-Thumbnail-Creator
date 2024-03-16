// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using SkiaSharp;

namespace BulkThumbnailCreator
{
    internal static class SkiaMethods
    {
        public static string CreateTextImage(string text, string font, int height, int width, Guid filename)
        {
            SKImageInfo info = new(width, height);

            // Create a SKSurface to draw on using the SKImageInfo
            using (var surface = SKSurface.Create(info))
            {
                // Get the canvas from the surface
                var canvas = surface.Canvas;

                // Create a paint object for drawing text
                using SKPaint textPaint = new();
                // Set text size based on box dimensions
                var textSize = CalculateTextSizeToFitBox(text, width, height);
                textPaint.TextSize = textSize;

                // Set other text paint properties
                textPaint.Typeface = SKTypeface.FromFamilyName(font);
                textPaint.Style = SKPaintStyle.Stroke; // Default
                textPaint.StrokeWidth = 8;

                // Create a random number generator
                Random random = new();

                // Generate random start and end colors for gradient
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

                // Measure the text bounds
                SKRect bounds = new();
                textPaint.MeasureText(text, ref bounds);

                // Calculate the position to center the text
                var x = (width - bounds.Width) / 2;
                var y = ((height + bounds.Height) / 2) - bounds.Bottom;

                // Draw the text on the canvas
                canvas.DrawText(text, x, y, textPaint);

                // Encode the surface as a PNG image
                using var skImage = surface.Snapshot();
                using var data = skImage.Encode();
                using Stream stream = File.OpenWrite(filename.ToString() + ".png");
                data.SaveTo(stream);
            }
            return filename.ToString() + ".png";
        }

        // Helper method to calculate text size to fit the box
        private static float CalculateTextSizeToFitBox(string text, int width, int height)
        {
            using SKPaint textPaint = new();
            textPaint.TextSize = 100; // Initial estimation size

            // Measure the text bounds using a temporary paint object
            SKRect bounds = new();
            textPaint.MeasureText(text, ref bounds);

            // Calculate the scaling factor to fit the text within the box
            var scaleX = width / bounds.Width;
            var scaleY = height / bounds.Height;
            var scale = Math.Min(scaleX, scaleY);

            // Adjust text size based on the scaling factor
            return textPaint.TextSize * scale;
        }
    }
}
