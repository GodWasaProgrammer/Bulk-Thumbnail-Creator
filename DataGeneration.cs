using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Bulk_Thumbnail_Creator
{
    public class DataGeneration
    {
        public static void DecideIfTooMuchFace(string CurrentFile, Bitmap PictureWhereFacesWereDetected, Rectangle[] rectangleArray)
        {
            int SumOfRectanglesX = 0;
            int SumOfRectanglesY = 0;

            foreach (Rectangle currentrectangle in rectangleArray)
            {
                SumOfRectanglesX += currentrectangle.X;
                SumOfRectanglesY += currentrectangle.Y;
            }

            int HalfOfPicHeight = PictureWhereFacesWereDetected.Height / 2;
            int HalfofPicWidth = PictureWhereFacesWereDetected.Width / 2;

            if (SumOfRectanglesX > HalfofPicWidth && SumOfRectanglesY > HalfOfPicHeight)
            {
                Settings.DiscardedBecauseTooMuchFacePictureData.Add(CurrentFile);
            }

        }

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

            float hueStrokeColor = ColorWheelSpinner(hueFillColor);
            float hueBorderColor = ColorWheelSpinner(hueFillColor);

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
        /// Picks a random font from the provided font folder
        /// </summary>
        /// <returns></returns>
        public static string PickRandomFont()
        {
            var fontNames = Directory.GetFiles("Fonts", "*.TTF*");

            Random randompicker = new();

            int randommax = fontNames.Length;

            int fontChosen = randompicker.Next(randommax);

            return fontNames[fontChosen].ToString();
        }

        /// <summary>
        /// Produces the remaining boxes of a picturedata object to create variety of choice
        /// </summary>
        /// <param name="PicToVarietize">The Target picture to varietize</param>
        /// <param name="TargetFolder">The target folder to varietize</param>
        public static void GenPlacementOfTextVariety(PictureData PicToVarietize)
        {
            Box CurrentBox = PicToVarietize.ParamForTextCreation.CurrentBox;

            if (PicToVarietize.ParamForTextCreation.Boxes.Count > 2)
            {
                foreach (var CurrentIterationBox in PicToVarietize.ParamForTextCreation.Boxes)
                {
                    PictureData CopiedPictureData = new(PicToVarietize);
                    CopiedPictureData.Varieties.Clear();

                    if (CurrentIterationBox.Key != CurrentBox)
                    {
                        // lift Rectangle
                        Rectangle currentRectangle = CurrentIterationBox.Value;

                        // write it to a Point
                        Point CurrentPoint = new(currentRectangle.X, currentRectangle.Y);

                        // feed it back into object
                        CopiedPictureData.ParamForTextCreation.PositionOfText = CurrentPoint;

                        // set the currentbox to the currentbox key
                        if (CurrentIterationBox.Key != CurrentBox)
                        {
                            CopiedPictureData.ParamForTextCreation.CurrentBox = CurrentIterationBox.Key;
                        }

                        Bitmap sourcePicture = new(CopiedPictureData.FileName);

                        // calculate box data, important for TextSettingsGeneration returning a correct ReadSettings object
                        CopiedPictureData.ParamForTextCreation = CalculateBoxData(CurrentIterationBox.Key, sourcePicture, CopiedPictureData.ParamForTextCreation);

                        // add it to list of created varieties
                        CopiedPictureData.OutPutType = OutputType.BoxPositionVariety;

                        PicToVarietize.Varieties.Add(CopiedPictureData);
                    }

                }
            }

        }

        /// <summary>
        /// This Allows you to Get the Text Position, It will Calculate where faces are
        /// and give you a position that is not colliding with a face
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sourcePicture"></param>
        /// <param name="faceRect"></param>
        /// <returns></returns>
        public static ParamForTextCreation GettextPos(ParamForTextCreation parameters, Bitmap sourcePicture, Rectangle[] faceRect)
        {
            Dictionary<Box, Rectangle> Boxes = new();

            #region Box Declarations
            // top box
            int TopBoxValueX = 0;
            int TopBoxValueY = 0;
            Box topBox = Box.TopBox;

            Rectangle TopBoxRectangle = new()
            {
                X = TopBoxValueX,
                Y = TopBoxValueY,
                Width = sourcePicture.Width,
                Height = TopBoxValueY
            };

            Boxes.Add(topBox, TopBoxRectangle);

            // bottom box
            int bottomBoxValueX = 0;
            int bottomBoxValueY = sourcePicture.Height / 2;
            Box bottomBox = Box.BottomBox;

            Rectangle bottomBoxRectangle = new()
            {
                X = bottomBoxValueX,
                Y = bottomBoxValueY,
                Width = sourcePicture.Width,
                Height = bottomBoxValueY
            };

            Boxes.Add(bottomBox, bottomBoxRectangle);

            // top left box
            int topLeftBoxValueX = 0;
            int topLeftBoxValueY = 0;
            Box topLeftBox = Box.TopLeft;

            Rectangle topLeftBoxRectangle = new()
            {
                X = topLeftBoxValueX,
                Y = topLeftBoxValueY,
                Width = sourcePicture.Width / 2,
                Height = topLeftBoxValueY / 2
            };

            Boxes.Add(topLeftBox, topLeftBoxRectangle);

            // top right box
            int topRightBoxValueX = sourcePicture.Width / 2;
            int topRightBoxValueY = 0;
            Box topRightBox = Box.TopRight;

            Rectangle topRightBoxRectangle = new()
            {
                X = topRightBoxValueX,
                Y = topRightBoxValueY,
                Width = sourcePicture.Width / 2,
                Height = sourcePicture.Height / 2
            };

            Boxes.Add(topRightBox, topRightBoxRectangle);

            // bottom left box
            int bottomLeftBoxValueX = 0;
            int bottomLeftBoxValueY = sourcePicture.Height / 2;
            Box bottomLeftBox = Box.BottomLeft;

            Rectangle bottomleftBoxRectangle = new()
            {
                X = bottomLeftBoxValueX,
                Y = bottomLeftBoxValueY,
                Width = sourcePicture.Width / 2,
                Height = sourcePicture.Height / 2
            };

            Boxes.Add(bottomLeftBox, bottomleftBoxRectangle);

            // bottom right box
            int bottomRightBoxValueX = sourcePicture.Width / 2;
            int bottomRightBoxValueY = sourcePicture.Height / 2;
            Box bottomRightBox = Box.BottomRight;

            Rectangle bottomRightBoxRectangle = new()
            {
                X = bottomRightBoxValueX,
                Y = bottomRightBoxValueY,
                Width = sourcePicture.Width / 2,
                Height = sourcePicture.Height / 2
            };

            Boxes.Add(bottomRightBox, bottomRightBoxRectangle);
            #endregion

            if (faceRect.Length != 0)
            {
                foreach (var face in faceRect)
                {
                    // handles the calculation of faces if the boxes are top/bottom boxes
                    int LocationOfRectangleCenterYpos = face.Y + face.Height / 2;

                    // sets the position to the middle of the picture, mid point at X = 0
                    int sourceIMGMiddleY = sourcePicture.Height / 2;

                    // if middle of image is more then the location of the rectangle height position 
                    if (sourceIMGMiddleY < LocationOfRectangleCenterYpos)
                    {
                        Boxes.Remove(bottomBox);
                    }

                    // if middle of image is less then the location of the rectangle height position
                    if (sourceIMGMiddleY > LocationOfRectangleCenterYpos)
                    {
                        Boxes.Remove(topBox);
                    }

                    // calculate position of face rectangle in relation to boxes
                    int MidPointX = sourcePicture.Width / 2;
                    int MidPointY = sourcePicture.Height / 2;

#pragma warning disable CA1853
                    if (!Boxes.ContainsKey(topBox))
                    {
                        Boxes.Remove(topLeftBox);
                        Boxes.Remove(topRightBox);
                    }
                    else
                    {
                        // if top left cornerbox face detected
                        if (face.X < MidPointX && face.Y < MidPointY)
                        {
                            Boxes.Remove(topLeftBox);
                        }
                        // if toprightbox face detected
                        if (face.X > MidPointX && face.Y < MidPointY)
                        {
                            Boxes.Remove(topRightBox);
                        }

                    }
#pragma warning disable CA1853
                    if (!Boxes.ContainsKey(bottomBox))
                    {
                        Boxes.Remove(bottomLeftBox);
                        Boxes.Remove(bottomRightBox);
                    }
                    else
                    {

                        // if bottomleftbox face detected
                        if (face.X < MidPointX && face.Y > MidPointY)
                        {
                            Boxes.Remove(bottomLeftBox);
                        }
                        // if bottomrightbox face detected
                        if (face.X > MidPointX && face.Y > MidPointY)
                        {
                            Boxes.Remove(bottomRightBox);
                        }

                    }

                }

            }

#pragma warning restore CA1853
            // write surviving boxvalues to object
            parameters.Boxes = Boxes;

            if (parameters.Boxes.Count != 0)
            {
                // all calculations done, pick one box
                Random random = new();
                Box pickedBoxName;

                // picks a random box that has no face in it
                do
                {
                    pickedBoxName = (Box)random.Next(parameters.Boxes.Count);
                }
                while (!parameters.Boxes.ContainsKey(pickedBoxName));

                // tries to read from dictionary
                parameters.Boxes.TryGetValue(pickedBoxName, out Rectangle pickedBoxRectangle);

                parameters.CurrentBox = pickedBoxName;

                // makes a point to feed to the parameters passed in
                Point pickedBoxPoint = new(pickedBoxRectangle.X, pickedBoxRectangle.Y);

                // sets the position of the parameter objects point variable 
                parameters.PositionOfText = pickedBoxPoint;

                // passes the box width and height to the parameter passed to the method
                // this is important to avoid boxes being juxtaposed outside of the image
                parameters = CalculateBoxData(pickedBoxName, sourcePicture, parameters);
            }

            parameters.Boxes = Boxes;
            return parameters;
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

            magickColor = RandomizeColor();
            param.StrokeColor.SetByRGB((byte)magickColor.R, (byte)magickColor.G, (byte)magickColor.B);

            magickColor = RandomizeColor();
            param.BorderColor.SetByRGB((byte)(magickColor.R), (byte)(magickColor.G), (byte)(magickColor.B));

            return param;
        }

        /// <summary>
        /// Produces 5 varieties of the first randomized font that hasnt already been chosen
        /// </summary>
        /// <param name="PicToVarietize">Your input PictureData object to produce varieties of</param>
        /// <param name="TargetFolder">The target folder of your object</param>
        public static void GenFontVariety(PictureData PicToVarietize)
        {
            List<string> fontList = new()
            {
                PicToVarietize.ParamForTextCreation.Font
            };

            int FontsToPick = 5;

            for (int i = 0; i < FontsToPick; i++)
            {
                string pickedFont = PickRandomFont();

                // if the list doesnt contain this font already, add it.
                if (!fontList.Contains(pickedFont))
                {
                    fontList.Add(pickedFont);
                }
                else
                {
                    i--;
                }

            }
            // variety selection finished, proceed to creating

            foreach (string font in fontList)
            {
                PictureData createFontVariety = new(PicToVarietize);
                createFontVariety.Varieties.Clear();

                createFontVariety.ParamForTextCreation.Font = font;
                createFontVariety.OutPutType = OutputType.FontVariety;
                PicToVarietize.Varieties.Add(createFontVariety);
            }

        }

        /// <summary>
        /// Support method to calculate where the box will be juxtaposed
        /// </summary>
        /// <param name="CurrentBox">The Box to calculate</param>
        /// <param name="sourcePicture">The Picture where the box is to be placed</param>
        /// <param name="CurrentParamForText">the current parameters for text creation input</param>
        /// <returns></returns>
        public static ParamForTextCreation CalculateBoxData(Box CurrentBox, Bitmap sourcePicture, ParamForTextCreation CurrentParamForText)
        {
            if (CurrentBox == Box.TopBox || CurrentBox == Box.BottomBox)
            {
                CurrentParamForText.WidthOfBox = sourcePicture.Width;
                CurrentParamForText.HeightOfBox = sourcePicture.Height / 2;
            }
            else
            {
                CurrentParamForText.WidthOfBox = sourcePicture.Width / 2;
                CurrentParamForText.HeightOfBox = sourcePicture.Height / 2;
            }

            return CurrentParamForText;
        }

        /// <summary>
        /// Creates Variety from an existing image, this will be on user interaction
        /// </summary>
        /// <param name="PictureInputData">The Image to create variety of</param>
        public static void GenSaturationVariety(PictureData PictureInputData)
        {
            const float baseLuminanceValue = 0.50F;
            float fillcolorHue = PictureInputData.ParamForTextCreation.FillColor.Hue;
            float fillcolorLuminance = baseLuminanceValue;

            float strokecolorHue = PictureInputData.ParamForTextCreation.StrokeColor.Hue;
            float strokecolorLuminance = baseLuminanceValue;

            float bordercolorHue = PictureInputData.ParamForTextCreation.BorderColor.Hue;
            float bordercolorLuminance = baseLuminanceValue;

            // create variety based on the current value
            List<float> VarietyList = new();

            float Variety1 = 0.30F;
            VarietyList.Add(Variety1);

            float Variety2 = 0.45F;
            VarietyList.Add(Variety2);

            float Variety3 = 0.55F;
            VarietyList.Add(Variety3);

            float Variety4 = 0.85F;
            VarietyList.Add(Variety4);

            float Variety5 = 1F;
            VarietyList.Add(Variety5);

            foreach (float variety in VarietyList)
            {
                PictureData VarietyData = new(PictureInputData);
                VarietyData.Varieties.Clear();

                VarietyData.ParamForTextCreation.FillColor.SetByHSL(fillcolorHue, variety, fillcolorLuminance);

                VarietyData.ParamForTextCreation.StrokeColor.SetByHSL(strokecolorHue, variety, strokecolorLuminance);

                VarietyData.ParamForTextCreation.BorderColor.SetByHSL(bordercolorHue, variety, bordercolorLuminance);

                Bitmap src = new(VarietyData.FileName);

                VarietyData.ParamForTextCreation = CalculateBoxData(VarietyData.ParamForTextCreation.CurrentBox, src, VarietyData.ParamForTextCreation);

                VarietyData.OutPutType = OutputType.SaturationVariety;
                PictureInputData.Varieties.Add(VarietyData);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PictureInputData"></param>
        public static void GenRandomVariety(PictureData PictureInputData)
        {
            const int NumberOfRandomsToProduce = 15;

            for (int CurrentIndex = 0; CurrentIndex < NumberOfRandomsToProduce; CurrentIndex++)
            {
                Random random = new();
                PictureData VarietyData = new(PictureInputData);
                VarietyData.Varieties.Clear();
                Dictionary<Box, Rectangle> AvailableBoxes = PictureInputData.ParamForTextCreation.Boxes;

                Box PickedBox;

                var availableBoxesExceptCurrent = AvailableBoxes.Keys.Where(box => box != PictureInputData.ParamForTextCreation.CurrentBox)
                                                                     .ToList();

                if (availableBoxesExceptCurrent.Count > 0)
                {
                    int randomIndex = random.Next(availableBoxesExceptCurrent.Count);
                    PickedBox = availableBoxesExceptCurrent[randomIndex];
                }
                else
                {
                    PickedBox = VarietyData.ParamForTextCreation.CurrentBox;
                }

                //do
                //{ //TODO: dont rely on RNG luck lol for it to work
                //    PickedBox = (Box)random.Next(PictureInputData.ParamForTextCreation.Boxes.Count);
                //}
                //while (!AvailableBoxes.ContainsKey(PickedBox));

                //foreach (Box CurrentIterationBox in AvailableBoxes.Keys)
                //{
                //    if (CurrentIterationBox != PictureInputData.ParamForTextCreation.CurrentBox)
                //    {
                //        PickedBox = CurrentIterationBox;
                //        break;
                //    }

                //}

                if (PickedBox != VarietyData.ParamForTextCreation.CurrentBox)
                {
                    VarietyData.ParamForTextCreation.CurrentBox = PickedBox;
                }


                Bitmap src = new(PictureInputData.FileName);

                VarietyData.ParamForTextCreation = CalculateBoxData(VarietyData.ParamForTextCreation.CurrentBox, src, VarietyData.ParamForTextCreation);

                VarietyData.ParamForTextCreation = GenerateRandomColorSettings(VarietyData.ParamForTextCreation);

                string Font = PickRandomFont();
                VarietyData.ParamForTextCreation.Font = Font;

                VarietyData.OutPutType = OutputType.RandomVariety;
                // VarietyData.OutPath = $"{CurrentIndex}"; // this is super bad form u donkey

                PictureInputData.Varieties.Add(VarietyData);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DankifyTarget"></param>
        public static void GenMemePosition(PictureData DankifyTarget)
        {
            PictureData CopiedPicData = new(DankifyTarget);
            CopiedPicData.Varieties.Clear();

            if (CopiedPicData.ParamForTextCreation.Boxes.Count > 2)
            {
                Dictionary<Box, Rectangle> boxesDictionary = DankifyTarget.ParamForTextCreation.Boxes;

                Box currentBox = DankifyTarget.ParamForTextCreation.CurrentBox;

                Box PickedBox = currentBox;

                List<Box> AvailableBoxes;

                AvailableBoxes = boxesDictionary.Keys.ToList();

                foreach (Box key in AvailableBoxes)
                {

                    if (currentBox == Box.BottomBox)
                    {
                        // avail boxes is only topright topleft
                        if (key == Box.TopLeft || key == Box.TopRight)
                        {
                            PickedBox = key;
                        }

                    }
                    if (currentBox == Box.TopBox)
                    {
                        // only possible boxes are bot left bot right
                        if (key == Box.BottomLeft || key == Box.BottomRight)
                        {
                            PickedBox = key;
                        }

                    }

                    if (currentBox == Box.TopLeft)
                    {
                        if (key == Box.TopRight || key == Box.BottomLeft || key == Box.BottomRight)
                        {
                            PickedBox = key;
                        }

                    }
                    if (currentBox == Box.TopRight)
                    {
                        if (key == Box.TopLeft || key == Box.BottomRight || key == Box.BottomLeft)
                        {
                            PickedBox = key;
                        }

                    }

                    if (currentBox == Box.BottomLeft)
                    {
                        if (key == Box.BottomRight || key == Box.TopLeft || key == Box.TopRight)
                        {
                            PickedBox = key;
                        }

                    }
                    if (currentBox == Box.BottomRight)
                    {
                        if (key == Box.BottomLeft || key == Box.TopLeft || key == Box.TopRight)
                        {
                            PickedBox = key;
                        }

                    }

                }

                if (PickedBox != currentBox)
                {
                    // pick a meme
                    Random pickRandomMeme = new();
                    int PickedMeme = pickRandomMeme.Next(Settings.Memes.Length);

                    //// write our chosen meme to Meme property
                    CopiedPicData.Meme = Settings.Memes[PickedMeme];

                    // set the type of output
                    CopiedPicData.OutPutType = OutputType.Dankness;

                    DankifyTarget.Varieties.Add(CopiedPicData);
                }

            }

        }

    }

}