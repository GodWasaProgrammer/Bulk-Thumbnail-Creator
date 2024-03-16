// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Ignore Spelling: Rgb

namespace BulkThumbnailCreator.PictureClasses;

public class Rgb(byte red, byte green, byte blue)
{
    public byte Red { get; set; } = red;
    public byte Green { get; set; } = green;
    public byte Blue { get; set; } = blue;
}
