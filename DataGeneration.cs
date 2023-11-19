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

            foreach (var box in Boxes)
            {
                Box PassBox = new();
                Boxes.TryGetValue(box.Key, out Rectangle Box);
                PassBox.Width = Box.Width;
                PassBox.Height = Box.Height;
                PassBox.X = Box.X;
                PassBox.Y = Box.Y;
                PassBox.Type = box.Key;
                parameters.Boxes.Add(PassBox);
            }

            // parameters.Boxes = Boxes;

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
                        bool BoxIntersect = Box.IntersectsWith(face);
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

                    bool BoxIntersect = Box.IntersectsWith(BoxToCheckIntersect);
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

                box.X = pickedBoxRectangle.X;
                box.Y = pickedBoxRectangle.Y;
                box.Rectangle = pickedBoxRectangle;
                box.Type = pickedBoxName;

                //parameters.CurrentBox.Type = pickedBoxName;

                parameters.CurrentBox = box;

                parameters.WidthOfBox = pickedBoxRectangle.Width;
                parameters.HeightOfBox = pickedBoxRectangle.Height;

                // makes a point to feed to the parameters passed in
                Point pickedBoxPoint = new(pickedBoxRectangle.X, pickedBoxRectangle.Y);

                // sets the position of the parameter objects point variable 
                parameters.PositionOfText = pickedBoxPoint;

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

                        Bitmap src = new(VarietyData.FileName);

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

                    Bitmap src = new(PictureInputData.FileName);

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
        //public static PictureData GenMemePosition(PictureData DankifyTarget)
        //{
        //    PictureData CopiedPicData = new(DankifyTarget);
        //    CopiedPicData.Varieties.Clear();

        //    for (int numberOfBoxes = 0; numberOfBoxes < DankifyTarget.NumberOfBoxes; numberOfBoxes++)
        //    {

        //        if (CopiedPicData.BoxParameters[numberOfBoxes].Boxes.Count > 2)
        //        {
        //            Dictionary<BoxType, Rectangle> boxesDictionary = DankifyTarget.BoxParameters[numberOfBoxes].Boxes;

        //            BoxType currentBox = DankifyTarget.BoxParameters[numberOfBoxes].CurrentBox;

        //            BoxType PickedBox = BoxType.None;

        //            List<BoxType> AvailableBoxes;

        //            AvailableBoxes = boxesDictionary.Keys.ToList();

        //            AvailableBoxes.Remove(currentBox);

        //            foreach (BoxType key in AvailableBoxes)
        //            {

        //                if (currentBox == BoxType.BottomBox)
        //                {
        //                    // avail boxes is only topright topleft
        //                    if (key == BoxType.TopLeft || key == BoxType.TopRight)
        //                    {
        //                        PickedBox = key;
        //                    }

        //                }

        //                if (currentBox == BoxType.TopBox)
        //                {
        //                    // only possible boxes are bot left bot right
        //                    if (key == BoxType.BottomLeft || key == BoxType.BottomRight)
        //                    {
        //                        PickedBox = key;
        //                    }
        //                }

        //                if (currentBox == BoxType.TopLeft)
        //                {
        //                    if (key == BoxType.TopRight || key == BoxType.BottomLeft || key == BoxType.BottomRight)
        //                    {
        //                        PickedBox = key;
        //                    }

        //                }

        //                if (currentBox == BoxType.TopRight)
        //                {
        //                    if (key == BoxType.TopLeft || key == BoxType.BottomRight || key == BoxType.BottomLeft)
        //                    {
        //                        PickedBox = key;
        //                    }

        //                }

        //                if (currentBox == BoxType.BottomLeft)
        //                {
        //                    if (key == BoxType.BottomRight || key == BoxType.TopLeft || key == BoxType.TopRight)
        //                    {
        //                        PickedBox = key;
        //                    }

        //                }

        //                if (currentBox == BoxType.BottomRight)
        //                {
        //                    if (key == BoxType.BottomLeft || key == BoxType.TopLeft || key == BoxType.TopRight)
        //                    {
        //                        PickedBox = key;
        //                    }

        //                }

        //            }

        //            if (PickedBox != BoxType.None)
        //            {
        //                // pick a meme
        //                Random pickRandomMeme = new();
        //                int PickedMeme = pickRandomMeme.Next(Settings.Memes.Length);

        //                //// write our chosen meme to Meme property
        //                CopiedPicData.BoxParameters[numberOfBoxes].Meme = Settings.Memes[PickedMeme];

        //                // set the type of output
        //                CopiedPicData.OutPutType = OutputType.Dankness;

        //                //DankifyTarget.Varieties.Add(CopiedPicData);
        //            }

        //        }
        //    }
        //    return CopiedPicData;
        //}

    }

}