﻿using BulkThumbnailCreator.Enums;
using BulkThumbnailCreator.PictureObjects;
using DlibDotNet;
using ImageMagick;

namespace BulkThumbnailCreator
{
    public class DataGeneration
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
        //public static void GenPlacementOfTextVariety(PictureData PicToVarietize)
        //{
        //    for (int boxParam = 0; boxParam < PicToVarietize.NumberOfBoxes; boxParam++)
        //    {
        //        BoxType CurrentBox = PicToVarietize.BoxParameters[boxParam].CurrentBox;

        //        if (PicToVarietize.BoxParameters[boxParam].Boxes.Count > 2)
        //        {
        //            foreach (var CurrentIterationBox in PicToVarietize.BoxParameters[boxParam].Boxes)
        //            {
        //                PictureData CopiedPictureData = new(PicToVarietize);
        //                CopiedPictureData.Varieties.Clear();

        //                if (CurrentIterationBox.Key != CurrentBox)
        //                {
        //                    // lift Rectangle
        //                    Rectangle currentRectangle = CurrentIterationBox.Value;

        //                    // write it to a Point
        //                    Point CurrentPoint = new(currentRectangle.X, currentRectangle.Y);

        //                    // feed it back into object
        //                    CopiedPictureData.BoxParameters[boxParam].PositionOfText = CurrentPoint;

        //                    // set the currentbox to the currentbox key
        //                    if (CurrentIterationBox.Key != CurrentBox)
        //                    {
        //                        CopiedPictureData.BoxParameters[boxParam].CurrentBox = CurrentIterationBox.Key;
        //                    }

        //                    Bitmap sourcePicture = new(CopiedPictureData.FileName);

        //                    // add it to list of created varieties
        //                    CopiedPictureData.OutPutType = OutputType.BoxPositionVariety;

        //                    PicToVarietize.Varieties.Add(CopiedPictureData);
        //                }

        //            }
        //        }

        //    }

        //}

        //private static Dictionary<BoxType, System.Drawing.Rectangle> BuildDefaultBoxesWindows(Dictionary<BoxType, System.Drawing.Rectangle> Boxes, Bitmap sourcePicture)
        //{
        //    // top box
        //    int TopBoxValueX = 0;
        //    int TopBoxValueY = 0;
        //    BoxType topBox = BoxType.TopBox;

        //    System.Drawing.Rectangle TopBoxRectangle = new()
        //    {
        //        X = TopBoxValueX,
        //        Y = TopBoxValueY,
        //        Width = sourcePicture.Width,
        //        Height = sourcePicture.Height / 2
        //    };

        //    Boxes.Add(topBox, TopBoxRectangle);

        //    // bottom box
        //    int bottomBoxValueX = 0;
        //    int bottomBoxValueY = sourcePicture.Height / 2;
        //    BoxType bottomBox = BoxType.BottomBox;

        //    System.Drawing.Rectangle bottomBoxRectangle = new()
        //    {
        //        X = bottomBoxValueX,
        //        Y = bottomBoxValueY,
        //        Width = sourcePicture.Width,
        //        Height = bottomBoxValueY
        //    };

        //    Boxes.Add(bottomBox, bottomBoxRectangle);

        //    // top left box
        //    int topLeftBoxValueX = 0;
        //    int topLeftBoxValueY = 0;
        //    BoxType topLeftBox = BoxType.TopLeft;

        //    System.Drawing.Rectangle topLeftBoxRectangle = new()
        //    {
        //        X = topLeftBoxValueX,
        //        Y = topLeftBoxValueY,
        //        Width = sourcePicture.Width / 2,
        //        Height = sourcePicture.Height / 2
        //    };

        //    Boxes.Add(topLeftBox, topLeftBoxRectangle);

        //    // top right box
        //    int topRightBoxValueX = sourcePicture.Width / 2;
        //    int topRightBoxValueY = 0;
        //    BoxType topRightBox = BoxType.TopRight;

        //    System.Drawing.Rectangle topRightBoxRectangle = new()
        //    {
        //        X = topRightBoxValueX,
        //        Y = topRightBoxValueY,
        //        Width = sourcePicture.Width / 2,
        //        Height = sourcePicture.Height / 2
        //    };

        //    Boxes.Add(topRightBox, topRightBoxRectangle);

        //    // bottom left box
        //    int bottomLeftBoxValueX = 0;
        //    int bottomLeftBoxValueY = sourcePicture.Height / 2;
        //    BoxType bottomLeftBox = BoxType.BottomLeft;

        //    System.Drawing.Rectangle bottomleftBoxRectangle = new()
        //    {
        //        X = bottomLeftBoxValueX,
        //        Y = bottomLeftBoxValueY,
        //        Width = sourcePicture.Width / 2,
        //        Height = sourcePicture.Height / 2
        //    };

        //    Boxes.Add(bottomLeftBox, bottomleftBoxRectangle);

        //    // bottom right box
        //    int bottomRightBoxValueX = sourcePicture.Width / 2;
        //    int bottomRightBoxValueY = sourcePicture.Height / 2;
        //    BoxType bottomRightBox = BoxType.BottomRight;

        //    System.Drawing.Rectangle bottomRightBoxRectangle = new()
        //    {
        //        X = bottomRightBoxValueX,
        //        Y = bottomRightBoxValueY,
        //        Width = sourcePicture.Width / 2,
        //        Height = sourcePicture.Height / 2
        //    };

        //    Boxes.Add(bottomRightBox, bottomRightBoxRectangle);

        //    return Boxes;
        //}

        /// <summary>
        /// This Allows you to Get the Text Position, It will Calculate where faces are
        /// and give you a position that is not colliding with a face
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sourcePicture"></param>
        /// <param name="faceRect"></param>
        /// <returns></returns>
        //public static ParamForTextCreation GettextPosWindows(ParamForTextCreation parameters, Bitmap sourcePicture, System.Drawing.Rectangle[] faceRect, PictureData pictureData = null)
        //{
        //    Dictionary<BoxType, System.Drawing.Rectangle> Boxes = new();

        //    Boxes = BuildDefaultBoxesWindows(Boxes, sourcePicture);

        //    foreach (var box in Boxes)
        //    {
        //        Box PassBox = new();
        //        Boxes.TryGetValue(box.Key, out System.Drawing.Rectangle Box);
        //        PassBox.Width = Box.Width;
        //        PassBox.Height = Box.Height;
        //        PassBox.X = Box.X;
        //        PassBox.Y = Box.Y;
        //        PassBox.Type = box.Key;
        //        parameters.Boxes.Add(PassBox);
        //    }

        //    // boxes that we will pick from after calcs done
        //    List<BoxType> FreeBoxes = Boxes.Keys.ToList();

        //    if (faceRect.Length != 0)
        //    {
        //        foreach (var face in faceRect)
        //        {
        //            List<(BoxType, bool)> FaceInterSectResults = new();

        //            foreach (BoxType boxType in Boxes.Keys)
        //            {
        //                Boxes.TryGetValue(boxType, out System.Drawing.Rectangle Box);
        //                bool BoxIntersect = Box.IntersectsWith(face);
        //                if (BoxIntersect)
        //                {
        //                    FreeBoxes.Remove(boxType);
        //                }
        //                FaceInterSectResults.Add((boxType, BoxIntersect));
        //            }

        //        }

        //    }

        //    // boxes to delete from the list of free boxes
        //    List<BoxType> BoxesToDelete = new();

        //    // needs to be delete because they are already occupied
        //    // or because they are the current box
        //    // or because they intersect
        //    for (int boxtodelete = 0; boxtodelete < pictureData.BoxParameters.Count; boxtodelete++)
        //    {
        //        if (FreeBoxes.Count > 0)
        //        {
        //            BoxesToDelete.Add(pictureData.BoxParameters[boxtodelete].CurrentBox.Type);
        //        }
        //    }

        //    // loops on our populated boxes and runs intersection checks and if they intersects deletes them
        //    foreach (var boxtype in BoxesToDelete)
        //    {
        //        Boxes.TryGetValue(boxtype, out System.Drawing.Rectangle BoxToCheckIntersect);
        //        List<(BoxType, bool)> IntersectResults = new();

        //        foreach (BoxType boxType in Boxes.Keys)
        //        {
        //            Boxes.TryGetValue(boxType, out System.Drawing.Rectangle Box);

        //            bool BoxIntersect = Box.IntersectsWith(BoxToCheckIntersect);
        //            if (BoxIntersect)
        //            {
        //                FreeBoxes.Remove(boxType);
        //            }
        //            IntersectResults.Add((boxType, BoxIntersect));
        //        }

        //    }

        //    // check what boxes are left
        //    if (FreeBoxes.Count != 0)
        //    {
        //        // all calculations done, pick one box if there are any left
        //        Random random = new();
        //        BoxType pickedBoxName;
        //        BoxType[] boxes = FreeBoxes.ToArray();

        //        pickedBoxName = boxes[random.Next(boxes.Length)];

        //        // tries to read from dictionary
        //        Boxes.TryGetValue(pickedBoxName, out System.Drawing.Rectangle pickedBoxRectangle);

        //        Box box = new();

        //        box.X = pickedBoxRectangle.X;
        //        box.Y = pickedBoxRectangle.Y;
        //        box.Width = pickedBoxRectangle.Width;
        //        box.Height = pickedBoxRectangle.Height;
        //        box.Rectangle = pickedBoxRectangle;
        //        box.Type = pickedBoxName;

        //        parameters.CurrentBox = box;

        //        parameters.WidthOfBox = pickedBoxRectangle.Width;
        //        parameters.HeightOfBox = pickedBoxRectangle.Height;

        //        FreeBoxes.Remove(pickedBoxName);

        //        parameters.BoxesWithNoFaceIntersect = FreeBoxes;

        //        // makes a point to feed to the parameters passed in
        //        System.Drawing.Point pickedBoxPoint = new(pickedBoxRectangle.X, pickedBoxRectangle.Y);

        //        // sets the position of the parameter objects point variable 
        //        parameters.PositionOfText = pickedBoxPoint;

        //    }

        //    return parameters;
        //}

        /// <summary>
        /// This Allows you to Get the Text Position, It will Calculate where faces are
        /// and give you a position that is not colliding with a face
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sourcePicture"></param>
        /// <param name="faceRect"></param>
        /// <returns></returns>
        public static ParamForTextCreation GettextPosLinux(ParamForTextCreation parameters, Array2D<RgbPixel> sourcePicture, Rectangle[] faceRect, PictureData pictureData = null)
        {
            Dictionary<BoxType, Rectangle> Boxes = new();

            Boxes = BuildDefaultBoxesLinux(Boxes, sourcePicture);

            foreach (var box in Boxes)
            {
                Box PassBox = new();
                Boxes.TryGetValue(box.Key, out Rectangle Box);
                PassBox.Width = (int)Box.Width;
                PassBox.Height = (int)Box.Height;
                PassBox.X = Box.Left;
                PassBox.Y = Box.Right;
                PassBox.Type = box.Key;
                parameters.Boxes.Add(PassBox);
            }

            // boxes that we will pick from after calcs done
            List<BoxType> FreeBoxes = Boxes.Keys.ToList();

            if (faceRect.Length != 0)
            {
                foreach (var face in faceRect)
                {
                    List<(BoxType, bool)> FaceInterSectResults = new();

                    foreach (BoxType boxType in Boxes.Keys)
                    {
                        Boxes.TryGetValue(boxType, out Rectangle Box);

                        bool BoxIntersect;

                        BoxIntersect = IntersectCheck(Box, face);

                        if (BoxIntersect)
                        {
                            FreeBoxes.Remove(boxType);
                        }
                        FaceInterSectResults.Add((boxType, BoxIntersect));
                    }

                }

            }

            // boxes to delete from the list of free boxes
            List<BoxType> BoxesToDelete = new();

            // needs to be delete because they are already occupied
            // or because they are the current box
            // or because they intersect
            for (int boxtodelete = 0; boxtodelete < pictureData.BoxParameters.Count; boxtodelete++)
            {
                if (FreeBoxes.Count > 0)
                {
                    BoxesToDelete.Add(pictureData.BoxParameters[boxtodelete].CurrentBox.Type);
                }
            }

            // loops on our populated boxes and runs intersection checks and if they intersects deletes them
            foreach (var boxtype in BoxesToDelete)
            {
                Boxes.TryGetValue(boxtype, out Rectangle BoxToCheckIntersect);
                List<(BoxType, bool)> IntersectResults = new();

                foreach (BoxType boxType in Boxes.Keys)
                {
                    Boxes.TryGetValue(boxType, out Rectangle Box);

                    bool BoxIntersect = IntersectCheck(Box, BoxToCheckIntersect);

                    if (BoxIntersect)
                    {
                        FreeBoxes.Remove(boxType);
                    }
                    IntersectResults.Add((boxType, BoxIntersect));
                }

            }

            // check what boxes are left
            if (FreeBoxes.Count != 0)
            {
                // all calculations done, pick one box if there are any left
                Random random = new();
                BoxType pickedBoxName;
                BoxType[] boxes = FreeBoxes.ToArray();

                pickedBoxName = boxes[random.Next(boxes.Length)];

                // tries to read from dictionary
                Boxes.TryGetValue(pickedBoxName, out Rectangle pickedBoxRectangle);

                Box box = new();

                box.X = pickedBoxRectangle.Left;
                box.Y = pickedBoxRectangle.Top;
                box.Width = (int)pickedBoxRectangle.Width;
                box.Height = (int)pickedBoxRectangle.Height;

                box.Rectangle = pickedBoxRectangle;

                box.Type = pickedBoxName;

                parameters.CurrentBox = box;

                parameters.WidthOfBox = (int)pickedBoxRectangle.Width;
                parameters.HeightOfBox = (int)pickedBoxRectangle.Height;

                FreeBoxes.Remove(pickedBoxName);

                parameters.BoxesWithNoFaceIntersect = FreeBoxes;

                // makes a point to feed to the parameters passed in
                Point pickedBoxPoint = new(pickedBoxRectangle.Left, pickedBoxRectangle.Top);

                // sets the position of the parameter objects point variable 
                parameters.PositionOfText = pickedBoxPoint;

            }

            return parameters;
        }

        public static bool IntersectCheck(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Left < rect2.Right &&
                   rect1.Right > rect2.Left &&
                   rect1.Top < rect2.Bottom &&
                   rect1.Bottom > rect2.Top;
        }

        private static Dictionary<BoxType, Rectangle> BuildDefaultBoxesLinux(Dictionary<BoxType, Rectangle> Boxes, Array2D<RgbPixel> sourcePicture)
        {
            // top box
            BoxType topBox = BoxType.TopBox;
            int width = sourcePicture.Columns;
            int height = sourcePicture.Rows / 2;

            Rectangle TopBoxRectangle = new(0, 0, width, height);

            Boxes.Add(topBox, TopBoxRectangle);
            //////////////////////////////////////////////

            //// bottom box
            BoxType bottomBox = BoxType.BottomBox;

            Rectangle bottomBoxRectangle = new(0, sourcePicture.Rows / 2, width, sourcePicture.Rows / 2);

            Boxes.Add(bottomBox, bottomBoxRectangle);

            // top left box
            BoxType topLeftBox = BoxType.TopLeft;

            width = sourcePicture.Columns / 2;
            height = sourcePicture.Rows / 2;

            Rectangle topLeftBoxRectangle = new(0, 0, width, height);

            Boxes.Add(topLeftBox, topLeftBoxRectangle);
            //////////////////////////////////////////////

            // top right box
            BoxType topRightBox = BoxType.TopRight;

            Rectangle topRightBoxRectangle = new(sourcePicture.Columns / 2, 0, sourcePicture.Columns, height);

            Boxes.Add(topRightBox, topRightBoxRectangle);
            //////////////////////////////////////////////

            // bottom left box
            BoxType bottomLeftBox = BoxType.BottomLeft;

            Rectangle bottomleftBoxRectangle = new(0, sourcePicture.Rows / 2, width, sourcePicture.Rows);

            Boxes.Add(bottomLeftBox, bottomleftBoxRectangle);
            //////////////////////////////////////////////

            // bottom right box
            BoxType bottomRightBox = BoxType.BottomRight;

            Rectangle bottomRightBoxRectangle = new(width, height, sourcePicture.Columns, sourcePicture.Rows);

            Boxes.Add(bottomRightBox, bottomRightBoxRectangle);
            //////////////////////////////////////////////

            return Boxes;
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
            for (int Box = 0; Box < PicToVarietize.BoxParameters.Count; Box++)
            {
                if (PicToVarietize.BoxParameters[Box].CurrentBox.Type == BoxType.None)
                {
                    break;
                }
                else
                {
                    List<string> fontList = new() { PicToVarietize.BoxParameters[Box].Font };

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

                        createFontVariety.BoxParameters[Box].Font = font;
                        createFontVariety.OutPutType = OutputType.FontVariety;
                        PicToVarietize.Varieties.Add(createFontVariety);
                    }

                }
            }
        }

        /// <summary>
        /// Creates Variety from an existing image, this will be on user interaction
        /// </summary>
        /// <param name="PictureInputData">The Image to create variety of</param>
        public static void GenSaturationVariety(PictureData PictureInputData)
        {
            for (int currentBox = 0; currentBox < PictureInputData.BoxParameters.Count; currentBox++)
            {
                if (PictureInputData.BoxParameters[currentBox].CurrentBox.Type == BoxType.None)
                {
                    break;
                }
                else
                {
                    const float baseLuminanceValue = 0.50F;
                    float fillColorHue = PictureInputData.BoxParameters[currentBox].FillColor.Hue;
                    float fillColorLuminance = baseLuminanceValue;

                    float strokeColorHue = PictureInputData.BoxParameters[currentBox].StrokeColor.Hue;
                    float strokeColorLuminance = baseLuminanceValue;

                    float borderColorHue = PictureInputData.BoxParameters[currentBox].BorderColor.Hue;
                    float borderColorLuminance = baseLuminanceValue;

                    // create variety based on the current value
                    List<float> VarietyList = new();

                    float Variety1 = 0.10F;
                    VarietyList.Add(Variety1);

                    float Variety2 = 0.25F;
                    VarietyList.Add(Variety2);

                    float Variety3 = 0.35F;
                    VarietyList.Add(Variety3);

                    float Variety4 = 0.65F;
                    VarietyList.Add(Variety4);

                    float Variety5 = 1F;
                    VarietyList.Add(Variety5);

                    foreach (float variety in VarietyList)
                    {
                        PictureData VarietyData = new(PictureInputData);
                        VarietyData.Varieties.Clear();

                        VarietyData.BoxParameters[currentBox].FillColor.SetByHSL(fillColorHue, variety, fillColorLuminance);

                        VarietyData.BoxParameters[currentBox].StrokeColor.SetByHSL(strokeColorHue, variety, strokeColorLuminance);

                        VarietyData.BoxParameters[currentBox].BorderColor.SetByHSL(borderColorHue, variety, borderColorLuminance);

                        //Bitmap src = new(VarietyData.FileName);

                        VarietyData.OutPutType = OutputType.SaturationVariety;
                        PictureInputData.Varieties.Add(VarietyData);
                    }

                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PictureInputData"></param>
        public static void GenRandomVariety(PictureData PictureInputData)
        {
            const int NumberOfRandomsToProduce = 2;

            for (int CurrentIndex = 0; CurrentIndex < NumberOfRandomsToProduce; CurrentIndex++)
            {
                for (int CurrentBoxes = 0; CurrentBoxes < PictureInputData.NumberOfBoxes; CurrentBoxes++)
                {
                    Random random = new();
                    PictureData VarietyData = new(PictureInputData);
                    VarietyData.Varieties.Clear();

                    // should perhaps have random boxes aswell

                    //Bitmap src = new(PictureInputData.FileName);

                    VarietyData.BoxParameters[CurrentBoxes] = GenerateRandomColorSettings(VarietyData.BoxParameters[CurrentBoxes]);

                    string Font = PickRandomFont();
                    VarietyData.BoxParameters[CurrentBoxes].Font = Font;

                    VarietyData.OutPutType = OutputType.RandomVariety;

                    PictureInputData.Varieties.Add(VarietyData);
                }

            }

        }

        public static void GenMemePosition(PictureData picDataToVar)
        {
            // call the copy ctor on the picdata object
            PictureData CopiedData = new(picDataToVar);

            // clear the variety list
            CopiedData.Varieties.Clear();

            /// make a list that collects the boxes of the current picdata object
            /// this is so we can determine which boxes are available to pick from
            List<BoxType> ReadBoxes = new();

            for (int i = 0; i < CopiedData.BoxParameters.Count; i++)
            {
                ReadBoxes.Add(CopiedData.BoxParameters[i].CurrentBox.Type);
            }

            // we will now determine if a meme is appropriate
            // it should only be considered appropriate if there is atleast
            // one additional box besides the meme box
            // if that is not the case, memes shouldnt be used
            // so if readboxes count is less then 2, skip this iteration

            if (ReadBoxes.Count == 2 || ReadBoxes.Count > 2)
            {
                for (int i = 0; i < ReadBoxes.Count; i++)
                {
                    // if we have looped and already made a meme, we need to make sure the text box
                    // also gets passed

                    if (CopiedData.OutPutType == OutputType.MemeVariety)
                    {
                        // dont modify object
                        // dont write any data to lists
                        // break out of loop
                        // this should ensure that we only make one meme per picture
                        // and that we dont end up with a meme and no text box
                    }
                    else
                    {
                        // pick a meme
                        Random pickRandomMeme = new();
                        int PickedMeme = pickRandomMeme.Next(Settings.Memes.Length);

                        // write our chosen meme to Meme property
                        CopiedData.BoxParameters[i].Meme = Settings.Memes[PickedMeme];
                        CopiedData.OutPutType = OutputType.MemeVariety;
                    }

                }

            }
            else
            {
                // do not modify picdata object
                // do not write any data to lists
                // break out of loop
            }

            // add the copied data to the list of varieties if all is successful
            picDataToVar.Varieties.Add(CopiedData);

        }

    }

}