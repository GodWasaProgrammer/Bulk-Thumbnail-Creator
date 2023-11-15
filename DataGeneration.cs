using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using UMapx.Imaging;

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
            for (int boxParam = 0; boxParam < PicToVarietize.NumberOfBoxes; boxParam++)
            {
                BoxType CurrentBox = PicToVarietize.BoxParameters[boxParam].CurrentBox;

                if (PicToVarietize.BoxParameters[boxParam].Boxes.Count > 2)
                {
                    foreach (var CurrentIterationBox in PicToVarietize.BoxParameters[boxParam].Boxes)
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
                            CopiedPictureData.BoxParameters[boxParam].PositionOfText = CurrentPoint;

                            // set the currentbox to the currentbox key
                            if (CurrentIterationBox.Key != CurrentBox)
                            {
                                CopiedPictureData.BoxParameters[boxParam].CurrentBox = CurrentIterationBox.Key;
                            }

                            Bitmap sourcePicture = new(CopiedPictureData.FileName);

                            // calculate box data, important for TextSettingsGeneration returning a correct ReadSettings object
                            CopiedPictureData.BoxParameters[boxParam] = CalculateBoxData(CurrentIterationBox.Key, sourcePicture, CopiedPictureData.BoxParameters[boxParam]);

                            // add it to list of created varieties
                            CopiedPictureData.OutPutType = OutputType.BoxPositionVariety;

                            PicToVarietize.Varieties.Add(CopiedPictureData);
                        }

                    }
                }

            }

            //Box CurrentBox = PicToVarietize.ParamForTextCreation.CurrentBox;

            //if (PicToVarietize.ParamForTextCreation.Boxes.Count > 2)
            //{
            //    foreach (var CurrentIterationBox in PicToVarietize.ParamForTextCreation.Boxes)
            //    {
            //        PictureData CopiedPictureData = new(PicToVarietize);
            //        CopiedPictureData.Varieties.Clear();

            //        if (CurrentIterationBox.Key != CurrentBox)
            //        {
            //            // lift Rectangle
            //            Rectangle currentRectangle = CurrentIterationBox.Value;

            //            // write it to a Point
            //            Point CurrentPoint = new(currentRectangle.X, currentRectangle.Y);

            //            // feed it back into object
            //            CopiedPictureData.ParamForTextCreation.PositionOfText = CurrentPoint;

            //            // set the currentbox to the currentbox key
            //            if (CurrentIterationBox.Key != CurrentBox)
            //            {
            //                CopiedPictureData.ParamForTextCreation.CurrentBox = CurrentIterationBox.Key;
            //            }

            //            Bitmap sourcePicture = new(CopiedPictureData.FileName);

            //            // calculate box data, important for TextSettingsGeneration returning a correct ReadSettings object
            //            CopiedPictureData.ParamForTextCreation = CalculateBoxData(CurrentIterationBox.Key, sourcePicture, CopiedPictureData.ParamForTextCreation);

            //            // add it to list of created varieties
            //            CopiedPictureData.OutPutType = OutputType.BoxPositionVariety;

            //            PicToVarietize.Varieties.Add(CopiedPictureData);
            //        }

            //    }
            //}

        }

        private static Dictionary<BoxType, Rectangle> BuildDefaultBoxes(Dictionary<BoxType, Rectangle> Boxes, Bitmap sourcePicture)
        {
            // top box
            int TopBoxValueX = 0;
            int TopBoxValueY = 0;
            BoxType topBox = BoxType.TopBox;

            Rectangle TopBoxRectangle = new()
            {
                X = TopBoxValueX,
                Y = TopBoxValueY,
                Width = sourcePicture.Width,
                Height = sourcePicture.Height / 2
            };

            Boxes.Add(topBox, TopBoxRectangle);

            // bottom box
            int bottomBoxValueX = 0;
            int bottomBoxValueY = sourcePicture.Height / 2;
            BoxType bottomBox = BoxType.BottomBox;

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
            BoxType topLeftBox = BoxType.TopLeft;

            Rectangle topLeftBoxRectangle = new()
            {
                X = topLeftBoxValueX,
                Y = topLeftBoxValueY,
                Width = sourcePicture.Width / 2,
                Height = sourcePicture.Height / 2
            };

            Boxes.Add(topLeftBox, topLeftBoxRectangle);

            // top right box
            int topRightBoxValueX = sourcePicture.Width / 2;
            int topRightBoxValueY = 0;
            BoxType topRightBox = BoxType.TopRight;

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
            BoxType bottomLeftBox = BoxType.BottomLeft;

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
            BoxType bottomRightBox = BoxType.BottomRight;

            Rectangle bottomRightBoxRectangle = new()
            {
                X = bottomRightBoxValueX,
                Y = bottomRightBoxValueY,
                Width = sourcePicture.Width / 2,
                Height = sourcePicture.Height / 2
            };

            Boxes.Add(bottomRightBox, bottomRightBoxRectangle);

            return Boxes;
        }

        // Returns true if two rectangles (l1, r1) 
        // and (l2, r2) overlap 
        private static bool DoOverlap(Point l1, Point r1, Point l2, Point r2)
        {
            // If one rectangle is on the left side or entirely to the left of the other
            if (l1.X >= r2.X || l2.X >= r1.X)
            {
                return false;
            }

            // If one rectangle is to the right or entirely to the right of the other
            if (r1.X <= l2.X || r2.X <= l1.X)
            {
                return false;
            }

            // If one rectangle is above or entirely above the other
            if (r1.Y <= l2.Y || r2.Y <= l1.Y)
            {
                return false;
            }

            // If one rectangle is below or entirely below the other
            if (l1.Y >= r2.Y || l2.Y >= r1.Y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This Allows you to Get the Text Position, It will Calculate where faces are
        /// and give you a position that is not colliding with a face
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sourcePicture"></param>
        /// <param name="faceRect"></param>
        /// <returns></returns>
        public static ParamForTextCreation GettextPos(ParamForTextCreation parameters, Bitmap sourcePicture, Rectangle[] faceRect, PictureData pictureData = null)
        {
            Dictionary<BoxType, Rectangle> Boxes = new();

            Boxes = BuildDefaultBoxes(Boxes, sourcePicture);
            parameters.Boxes = Boxes;

            // boxes that we will pick from after calcs done
            List<BoxType> FreeBoxes = Boxes.Keys.ToList();

            if (faceRect.Length != 0)
            {
                foreach (var face in faceRect)
                {
                    List<(BoxType, bool)> FaceInterSectResults = new();

                    Boxes.TryGetValue(BoxType.TopBox, out Rectangle TopBox);
                    bool TopBoxIntersect = TopBox.IntersectsWith(face);
                    if (TopBoxIntersect)
                    {
                        FreeBoxes.Remove(BoxType.TopBox);
                    }
                    FaceInterSectResults.Add((BoxType.TopBox, TopBoxIntersect));

                    Boxes.TryGetValue(BoxType.BottomBox, out Rectangle BottomBox);
                    bool BotBoxInterSect = BottomBox.IntersectsWith(face);
                    if (BotBoxInterSect)
                    {
                        FreeBoxes.Remove(BoxType.BottomBox);
                    }
                    FaceInterSectResults.Add((BoxType.BottomBox, BotBoxInterSect));

                    Boxes.TryGetValue(BoxType.TopRight, out Rectangle TopRightBox);
                    bool TopRightBoxInterSect = TopRightBox.IntersectsWith(face);
                    if (TopRightBoxInterSect)
                    {
                        FreeBoxes.Remove(BoxType.TopRight);
                    }
                    FaceInterSectResults.Add((BoxType.TopRight, TopRightBoxInterSect));

                    Boxes.TryGetValue(BoxType.TopLeft, out Rectangle TopLeftBox);
                    bool TopLeftBoxInterSect = TopLeftBox.IntersectsWith(face);
                    if (TopLeftBoxInterSect)
                    {
                        FreeBoxes.Remove(BoxType.TopLeft);
                    }
                    FaceInterSectResults.Add((BoxType.TopLeft, TopLeftBoxInterSect));

                    Boxes.TryGetValue(BoxType.BottomLeft, out Rectangle BottomLeftBox);
                    bool BottomLeftBoxInterSect = BottomLeftBox.IntersectsWith(face);
                    if (BottomLeftBoxInterSect)
                    {
                        FreeBoxes.Remove(BoxType.BottomLeft);
                    }
                    FaceInterSectResults.Add((BoxType.BottomLeft, BottomLeftBoxInterSect));


                    Boxes.TryGetValue(BoxType.BottomRight, out Rectangle BottomRightBox);
                    bool BottomRightBoxInterSect = BottomRightBox.IntersectsWith(face);
                    if (BottomRightBoxInterSect)
                    {
                        FreeBoxes.Remove(BoxType.BottomRight);
                    }
                    FaceInterSectResults.Add((BoxType.BottomRight, BottomRightBoxInterSect));

                }

            }

            // boxes to delete from the list of free boxes
            List<BoxType> BoxesToDelete = new();

            // needs to be delete because they are already occupied
            // or because they are the current box
            // or because they intersect
            for (int i = 0; i < pictureData.BoxParameters.Count; i++)
            {
                if (pictureData.BoxParameters[i].Boxes.Count > 0)
                {
                    BoxesToDelete.Add(pictureData.BoxParameters[i].CurrentBox);
                }
            }

            foreach (var box in BoxesToDelete)
            {
                if (FreeBoxes.Contains(box))
                {
                    FreeBoxes.Remove(box);
                }

                if (box == BoxType.BottomBox)
                {
                    FreeBoxes.Remove(BoxType.BottomLeft);
                    FreeBoxes.Remove(BoxType.BottomRight);
                }

                if (box == BoxType.TopBox)
                {
                    FreeBoxes.Remove(BoxType.TopLeft);
                    FreeBoxes.Remove(BoxType.TopRight);
                }

                if (box == BoxType.TopLeft)
                {
                    if (FreeBoxes.Contains(BoxType.TopBox))
                    {
                        FreeBoxes.Remove(BoxType.TopBox);
                    }
                }

                if (box == BoxType.TopRight)
                {
                    if (FreeBoxes.Contains(BoxType.TopBox))
                    {
                        FreeBoxes.Remove(BoxType.TopBox);
                    }
                }

                if (box == BoxType.BottomLeft)
                {
                    if (FreeBoxes.Contains(BoxType.BottomBox))
                    {
                        FreeBoxes.Remove(BoxType.BottomBox);
                    }
                }

                if (box == BoxType.BottomRight)
                {
                    if (FreeBoxes.Contains(BoxType.BottomBox))
                    {
                        FreeBoxes.Remove(BoxType.BottomBox);
                    }
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
                parameters.Boxes.TryGetValue(pickedBoxName, out Rectangle pickedBoxRectangle);

                parameters.CurrentBox = pickedBoxName;
                parameters.WidthOfBox = pickedBoxRectangle.Width;
                parameters.HeightOfBox = pickedBoxRectangle.Height;

                // makes a point to feed to the parameters passed in
                Point pickedBoxPoint = new(pickedBoxRectangle.X, pickedBoxRectangle.Y);

                // sets the position of the parameter objects point variable 
                parameters.PositionOfText = pickedBoxPoint;

                // passes the box width and height to the parameter passed to the method
                // this is important to avoid boxes being juxtaposed outside of the image
                // parameters = CalculateBoxData(pickedBoxName, sourcePicture, parameters);
            }

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
            for (int numberOfBoxes = 0; numberOfBoxes < PicToVarietize.NumberOfBoxes; numberOfBoxes++)
            {

                List<string> fontList = new()
            {
                PicToVarietize.BoxParameters[numberOfBoxes].Font
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

                    createFontVariety.BoxParameters[numberOfBoxes].Font = font;
                    createFontVariety.OutPutType = OutputType.FontVariety;
                    PicToVarietize.Varieties.Add(createFontVariety);
                }

            }
        }

        /// <summary>
        /// Support method to calculate where the box will be juxtaposed
        /// </summary>
        /// <param name="CurrentBox">The Box to calculate</param>
        /// <param name="sourcePicture">The Picture where the box is to be placed</param>
        /// <param name="CurrentParamForText">the current parameters for text creation input</param>
        /// <returns></returns>
        public static ParamForTextCreation CalculateBoxData(BoxType CurrentBox, Bitmap sourcePicture, ParamForTextCreation CurrentParamForText)
        {
            if (CurrentBox == BoxType.TopBox || CurrentBox == BoxType.BottomBox)
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
            for (int numberOfBoxes = 0; numberOfBoxes < PictureInputData.NumberOfBoxes; numberOfBoxes++)
            {
                const float baseLuminanceValue = 0.50F;
                float fillcolorHue = PictureInputData.BoxParameters[numberOfBoxes].FillColor.Hue;
                float fillcolorLuminance = baseLuminanceValue;

                float strokecolorHue = PictureInputData.BoxParameters[numberOfBoxes].StrokeColor.Hue;
                float strokecolorLuminance = baseLuminanceValue;

                float bordercolorHue = PictureInputData.BoxParameters[numberOfBoxes].BorderColor.Hue;
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

                    VarietyData.BoxParameters[numberOfBoxes].FillColor.SetByHSL(fillcolorHue, variety, fillcolorLuminance);

                    VarietyData.BoxParameters[numberOfBoxes].StrokeColor.SetByHSL(strokecolorHue, variety, strokecolorLuminance);

                    VarietyData.BoxParameters[numberOfBoxes].BorderColor.SetByHSL(bordercolorHue, variety, bordercolorLuminance);

                    Bitmap src = new(VarietyData.FileName);

                    VarietyData.BoxParameters[numberOfBoxes] = CalculateBoxData(VarietyData.BoxParameters[numberOfBoxes].CurrentBox, src, VarietyData.BoxParameters[numberOfBoxes]);

                    VarietyData.OutPutType = OutputType.SaturationVariety;
                    PictureInputData.Varieties.Add(VarietyData);
                }

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
                for (int CurrentBoxes = 0; CurrentBoxes < PictureInputData.NumberOfBoxes; CurrentBoxes++)
                {

                    Random random = new();
                    PictureData VarietyData = new(PictureInputData);
                    VarietyData.Varieties.Clear();
                    Dictionary<BoxType, Rectangle> AvailableBoxes = PictureInputData.BoxParameters[CurrentBoxes].Boxes;

                    BoxType PickedBox;

                    var availableBoxesExceptCurrent = AvailableBoxes.Keys.Where(box => box != PictureInputData.BoxParameters[CurrentBoxes].CurrentBox)
                                                                         .ToList();

                    if (availableBoxesExceptCurrent.Count > 0)
                    {
                        int randomIndex = random.Next(availableBoxesExceptCurrent.Count);
                        PickedBox = availableBoxesExceptCurrent[randomIndex];
                    }
                    else
                    {
                        PickedBox = VarietyData.BoxParameters[CurrentBoxes].CurrentBox;
                    }

                    if (PickedBox != VarietyData.BoxParameters[CurrentBoxes].CurrentBox)
                    {
                        VarietyData.BoxParameters[CurrentBoxes].CurrentBox = PickedBox;
                    }

                    Bitmap src = new(PictureInputData.FileName);

                    VarietyData.BoxParameters[CurrentBoxes] = CalculateBoxData(VarietyData.BoxParameters[CurrentBoxes].CurrentBox, src, VarietyData.BoxParameters[CurrentBoxes]);

                    VarietyData.BoxParameters[CurrentBoxes] = GenerateRandomColorSettings(VarietyData.BoxParameters[CurrentBoxes]);

                    string Font = PickRandomFont();
                    VarietyData.BoxParameters[CurrentBoxes].Font = Font;

                    VarietyData.OutPutType = OutputType.RandomVariety;

                    PictureInputData.Varieties.Add(VarietyData);
                }

            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DankifyTarget"></param>
        public static PictureData GenMemePosition(PictureData DankifyTarget)
        {
            PictureData CopiedPicData = new(DankifyTarget);
            CopiedPicData.Varieties.Clear();

            for (int numberOfBoxes = 0; numberOfBoxes < DankifyTarget.NumberOfBoxes; numberOfBoxes++)
            {

                if (CopiedPicData.BoxParameters[numberOfBoxes].Boxes.Count > 2)
                {
                    Dictionary<BoxType, Rectangle> boxesDictionary = DankifyTarget.BoxParameters[numberOfBoxes].Boxes;

                    BoxType currentBox = DankifyTarget.BoxParameters[numberOfBoxes].CurrentBox;

                    BoxType PickedBox = BoxType.None;

                    List<BoxType> AvailableBoxes;

                    AvailableBoxes = boxesDictionary.Keys.ToList();

                    AvailableBoxes.Remove(currentBox);

                    foreach (BoxType key in AvailableBoxes)
                    {

                        if (currentBox == BoxType.BottomBox)
                        {
                            // avail boxes is only topright topleft
                            if (key == BoxType.TopLeft || key == BoxType.TopRight)
                            {
                                PickedBox = key;
                            }

                        }

                        if (currentBox == BoxType.TopBox)
                        {
                            // only possible boxes are bot left bot right
                            if (key == BoxType.BottomLeft || key == BoxType.BottomRight)
                            {
                                PickedBox = key;
                            }
                        }

                        if (currentBox == BoxType.TopLeft)
                        {
                            if (key == BoxType.TopRight || key == BoxType.BottomLeft || key == BoxType.BottomRight)
                            {
                                PickedBox = key;
                            }

                        }

                        if (currentBox == BoxType.TopRight)
                        {
                            if (key == BoxType.TopLeft || key == BoxType.BottomRight || key == BoxType.BottomLeft)
                            {
                                PickedBox = key;
                            }

                        }

                        if (currentBox == BoxType.BottomLeft)
                        {
                            if (key == BoxType.BottomRight || key == BoxType.TopLeft || key == BoxType.TopRight)
                            {
                                PickedBox = key;
                            }

                        }

                        if (currentBox == BoxType.BottomRight)
                        {
                            if (key == BoxType.BottomLeft || key == BoxType.TopLeft || key == BoxType.TopRight)
                            {
                                PickedBox = key;
                            }

                        }

                    }

                    if (PickedBox != BoxType.None)
                    {
                        // pick a meme
                        Random pickRandomMeme = new();
                        int PickedMeme = pickRandomMeme.Next(Settings.Memes.Length);

                        //// write our chosen meme to Meme property
                        CopiedPicData.BoxParameters[numberOfBoxes].Meme = Settings.Memes[PickedMeme];

                        // set the type of output
                        CopiedPicData.OutPutType = OutputType.Dankness;

                        //DankifyTarget.Varieties.Add(CopiedPicData);
                    }

                }
            }
            return CopiedPicData;
        }

    }

}