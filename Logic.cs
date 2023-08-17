using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YoutubeDLSharp;
using System.Drawing;
using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using System.Reflection;

namespace Bulk_Thumbnail_Creator
{
    internal class Logic
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
                BTCSettings.DiscardedBecauseTooMuchFacePictureData.Add(CurrentFile);
            }

        }

        private static readonly Random colorRandom = new();

        /// <summary>
        /// Generates random colors in bytes
        /// </summary>
        /// <returns>returns a MagickColor Object which is RGB</returns>
        internal static MagickColor RandomizeColor()
        {
            byte pickedColorRedRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
            byte pickedColorGreenRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
            byte pickedColorBlueRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);

            MagickColor colorRNGPicked;

            colorRNGPicked = MagickColor.FromRgb(pickedColorRedRGB, pickedColorGreenRGB, pickedColorBlueRGB);

            return colorRNGPicked;
        }

        static float hueFillColor = 0F;
        static readonly float saturationFillColor = 1F;
        static readonly float lightnessFillColor = 0.50F;

        static float hueStrokeColor = 125F;
        static readonly float saturationStrokeColor = 1F;
        static readonly float lightnessStrokeColor = 0.50F;

        static float hueBorderColor = 28F;
        static readonly float saturationBorderColor = 1F;
        static readonly float lightnessBorderColor = 0.50F;

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
            const float maxHueValue = 360F;
            const float incrementalColor = 12.5F;
            const float resetFromMaxToMin = 0F;

            hueFillColor += incrementalColor;

            hueStrokeColor = ColorWheelSpinner(hueFillColor);
            hueBorderColor = ColorWheelSpinner(hueFillColor);

            if (hueFillColor > maxHueValue)
            {
                hueFillColor = resetFromMaxToMin;
            }

            InputParameter.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);

            InputParameter.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);

            InputParameter.BorderColor.SetByHSL(hueBorderColor, saturationBorderColor, lightnessBorderColor);

            return InputParameter;
        }

        static readonly XmlSerializer serializer = new(typeof(List<string>));

        /// <summary>
        /// General DeSerializer for List of Strings
        /// </summary>
        /// <param name="pathtoXMLToDeSerialize">The XML to be deserialized</param>
        /// <returns>The Deserialized List of Strings</returns>
        public static List<string> DeSerializeXMLToListOfStrings(string pathtoXMLToDeSerialize)
        {
            List<string> ListofStringsToDeSerialize = BTCSettings.DownloadedVideosList;

            if (File.Exists(pathtoXMLToDeSerialize))
            {
                using FileStream file = File.OpenRead(pathtoXMLToDeSerialize);
                ListofStringsToDeSerialize = (List<string>)serializer.Deserialize(file);

            }
            return ListofStringsToDeSerialize;
        }

        /// <summary>
        ///  General Serializer for list of strings
        /// </summary>
        /// <param name="PathToXML">Your Path to the XML to be written to</param>
        /// <param name="ListOfStringsToSerialize">The List of Strings to Serialize</param>
        public static void SerializeListOfStringsToXML(string PathToXML, List<string> ListOfStringsToSerialize)
        {
            using FileStream file = File.Create(PathToXML);
            serializer.Serialize(file, ListOfStringsToSerialize);

        }

        /// <summary>
        /// Instantiates a YoutubeDL and downloads the specified string
        /// </summary>
        /// <param name="URL">the specified link URL to download</param>
        /// <returns>returns the path to the downloaded video</returns>
        public static async Task<string> YouTubeDL(string URL)
        {
            string CurrentLoc = Assembly.GetExecutingAssembly().Location;
            string parentDirectory = Directory.GetParent(CurrentLoc).FullName;
            string newPath = Path.Combine(parentDirectory, "Executables");
            string YTDLPDir = Path.Combine(newPath, "yt-dlp.exe");
            string FfMpegDir = Path.Combine(newPath, "ffmpeg.exe");

            Console.WriteLine($"Current Location: {CurrentLoc}");
            Console.WriteLine($"Parent Directory: {parentDirectory}");
            Console.WriteLine($"New Path: {newPath}");
            if (File.Exists (YTDLPDir))
            {
            Console.WriteLine($"YTDLP Path: {YTDLPDir}");
            }
            else
            {
                Console.WriteLine("yt-dlp.exe was not found");
            }
            if (File.Exists (FfMpegDir)) 
            { 
            Console.WriteLine($"FFmpeg Path: {FfMpegDir}");
            }
            else
            {
                Console.WriteLine("ffmpeg.exe was not found");
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string ytdlDir = Path.Combine(currentDirectory, "YTDL");

            if (Directory.Exists (ytdlDir))
            {
                Console.WriteLine("YTDL Dir found");
            }
            else
            {
                Console.WriteLine("YTDL Dir was not found");
            }

            var ytdl = new YoutubeDL
            {
                // set paths
                YoutubeDLPath = YTDLPDir,
                FFmpegPath = FfMpegDir,
                OutputFolder = ytdlDir,
            };

            // downloads specified video from youtube if it does not already exist.
            //BTCSettings.YoutubeLink = URL;
            var res = await ytdl.RunVideoDownload(url: URL);
            await Console.Out.WriteLineAsync("Download Success:" + res.Success.ToString());

            // sets BTC to run on the recently downloaded file res.data is the returned path.
            return res.Data;
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
        /// Creates our directories for operations
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="TextAdded"></param>
        /// <param name="YTDL"></param>
        public static void CreateDirectories(string outputDir, string TextAdded, string YTDL)
        {
            Directory.CreateDirectory(outputDir);
            Directory.CreateDirectory(TextAdded);
            Directory.CreateDirectory(YTDL);
        }

        public static ParamForTextCreation GettextPos(ParamForTextCreation parameters, Bitmap sourcePicture, Rectangle[] faceRect)
        {
            Dictionary<Box, Rectangle> Boxes = new();

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

            // write surviving boxvalues to object
            parameters.Boxes = Boxes;

            if (Boxes.Count == 0)
            {
                parameters.CurrentBox = Box.Discarded;
            }
            else
            {
                // all calculations done, pick one box
                Random random = new();
                // picks a random box that has no face in it
                Box pickedBoxName = (Box)random.Next(Boxes.Count);

                // tries to read from dictionary
                Boxes.TryGetValue(pickedBoxName, out Rectangle pickedBoxRectangle);

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
        /// Produces 5 varieties of the first randomized font that hasnt already been chosen
        /// </summary>
        /// <param name="PicToVarietize">Your input PictureData object to produce varieties of</param>
        /// <param name="TargetFolder">The target folder of your object</param>
        public static void ProduceFontVarietyData(PictureData PicToVarietize)
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

                createFontVariety.ParamForTextCreation.Font = font;
                createFontVariety.OutputType = OutputType.FontVariety;
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
        /// Produces the remaining boxes of a picturedata object to create variety of choice
        /// </summary>
        /// <param name="PicToVarietize">The Target picture to varietize</param>
        /// <param name="TargetFolder">The target folder to varietize</param>
        public static void ProducePlacementOfTextVarietyData(PictureData PicToVarietize)
        {
            Box boxToExclude = PicToVarietize.ParamForTextCreation.CurrentBox;

            if (boxToExclude != Box.Discarded)
            {
                foreach (var CurrentBox in PicToVarietize.ParamForTextCreation.Boxes)
                {
                    PictureData CopiedPictureData = new(PicToVarietize);

                    if (CurrentBox.Key != boxToExclude)
                    {
                        // lift Rectangle
                        Rectangle currentRectangle = CurrentBox.Value;

                        // write it to a Point
                        Point CurrentPoint = new(currentRectangle.X, currentRectangle.Y);

                        // feed it back into object
                        CopiedPictureData.ParamForTextCreation.PositionOfText = CurrentPoint;

                        // set the currentbox to the currentbox key
                        if (CurrentBox.Key != Box.Discarded)
                        {
                            CopiedPictureData.ParamForTextCreation.CurrentBox = CurrentBox.Key;
                        }

                        Bitmap sourcePicture = new(CopiedPictureData.FileName);

                        // calculate box data, important for TextSettingsGeneration returning a correct ReadSettings object
                        CopiedPictureData.ParamForTextCreation = CalculateBoxData(CurrentBox.Key, sourcePicture, CopiedPictureData.ParamForTextCreation);

                        // add it to list of created varieties
                        CopiedPictureData.OutputType = OutputType.BoxPositionVariety;

                        PicToVarietize.Varieties.Add(CopiedPictureData);
                    }

                }
            }

        }

        /// <summary>
        /// Produces Picture using the ImageMagick Library
        /// </summary>
        /// <param name="PicData">PictureDataObject Containing everything needed to create an image</param>
        public static void ProduceTextPictures(PictureData PicData)
        {
            string imageName;
            string OutputPath = Path.GetFullPath(BTCSettings.TextAddedDir);

            imageName = Path.GetFileName(PicData.FileName);
            DateTime dateTime = DateTime.Now;
                string trimDateTime = dateTime.ToString();
            trimDateTime = trimDateTime.Replace(":", "");

            // if not main type, we will make a directory for files to be written in
            if (PicData.OutputType != OutputType.Main)
            {
                Directory.CreateDirectory(OutputPath + "//" + "variety of " + Path.GetFileName(PicData.FileName));
            }

            if (PicData.OutputType == OutputType.Main)
            {
                OutputPath += "//" + trimDateTime + imageName;
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.BoxPositionVariety)
            {
                OutputPath += "//" + "variety of " + Path.GetFileName(PicData.FileName) + $"//{PicData.ParamForTextCreation.CurrentBox}" + ".png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.SaturationVariety)
            {
                OutputPath += "//" + "variety of " + Path.GetFileName(PicData.FileName) + $"//{PicData.ParamForTextCreation.FillColor.Saturation} " + ".png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.FontVariety)
            {
                OutputPath += "//" + "variety of " + Path.GetFileName(PicData.FileName) + "//" + Path.GetFileNameWithoutExtension(PicData.ReadSettings.Font) + ".png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.RandomVariety)
            {
                OutputPath += "//" + "variety of " + Path.GetFileName(PicData.FileName) + "//" + $"{PicData.OutPath}" + ".png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.Dankness)
            {
                OutputPath += "//" + "variety of " + Path.GetFileName(PicData.FileName) + "//" + PicData.Dankbox + ".png";
                PicData.OutPath = OutputPath;
            }

            using MagickImage outputImage = new(Path.GetFullPath(PicData.FileName));
            using var caption = new MagickImage($"caption:{PicData.ParamForTextCreation.Text}", PicData.ReadSettings);

            // Add the caption layer on top of the background image
            if (PicData.ParamForTextCreation.Boxes.ContainsKey(PicData.ParamForTextCreation.CurrentBox))
            {
                int takeX = PicData.ParamForTextCreation.Boxes[PicData.ParamForTextCreation.CurrentBox].X;

                int takeY = PicData.ParamForTextCreation.Boxes[PicData.ParamForTextCreation.CurrentBox].Y;

                outputImage.Composite(caption, takeX, takeY, CompositeOperator.Over);
            }

            outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);

            if (PicData.OutputType == OutputType.Dankness)
            {
                MagickImage MemeToPutOnPic = new(PicData.Meme);

                Rectangle ReadPosition = PicData.ParamForTextCreation.Boxes[PicData.Dankbox];

                int posX = ReadPosition.X;
                int posY = ReadPosition.Y;

                outputImage.Composite(MemeToPutOnPic, posX, posY, CompositeOperator.Over);
            }
            outputImage.Quality = 100;

            // outputs the file to the provided path and name
            outputImage.Write(OutputPath);

        }

        /// <summary>
        /// Generates MagickReadSettings with completely randomized color values for Fill/stroke/border colors
        /// ALso generates the other necessary settings for ImageMagick necessary to put text on the screenshots
        /// </summary>
        /// <returns>returns the randomized MagickReadSettings object</returns>
        public static MagickReadSettings GenerateRandomColorSettings(ParamForTextCreation param)
        {
            MagickReadSettings settingsTextRandom = new()
            {
                Font = param.Font,
                FillColor = RandomizeColor(),
                StrokeColor = RandomizeColor(),
                BorderColor = RandomizeColor(),
                //FontStyle = FontStyleType.Bold,
                StrokeAntiAlias = true,
                StrokeWidth = 6,
                FontWeight = FontWeight.Bold,
                BackgroundColor = MagickColors.Transparent,
                Height = param.HeightOfBox, // height of text box
                Width = param.WidthOfBox, // width of text box
            };

            return settingsTextRandom;
        }

        public static void ProduceRandomVariety(PictureData PictureInputData)
        {
            const int NumberOfRandomsToProduce = 15;

            for (int CurrentIndex = 0; CurrentIndex < NumberOfRandomsToProduce; CurrentIndex++)
            {
                Random random = new();
                PictureData VarietyData = new(PictureInputData);

                Box PickedBox = (Box)random.Next(PictureInputData.ParamForTextCreation.Boxes.Count);

                VarietyData.ParamForTextCreation.CurrentBox = PickedBox;

                Bitmap src = new(PictureInputData.FileName);

                VarietyData.ParamForTextCreation = CalculateBoxData(VarietyData.ParamForTextCreation.CurrentBox, src, VarietyData.ParamForTextCreation);

                string Font = PickRandomFont();
                VarietyData.ParamForTextCreation.Font = Font;

                VarietyData.OutputType = OutputType.RandomVariety;
                VarietyData.OutPath = $"{CurrentIndex}";
                PictureInputData.Varieties.Add(VarietyData);
            }

        }

        public static void ProduceMemeDankness(PictureData DankifyTarget)
        {
            PictureData CopiedPicData = new(DankifyTarget);

            if (CopiedPicData.ParamForTextCreation.Boxes.Count > 2)
            {
                // pick a box to dankify
                Random pickbox = new();
                Box PickedBox = (Box)pickbox.Next(DankifyTarget.ParamForTextCreation.Boxes.Count);

                // write picked box to dankbox property
                CopiedPicData.Dankbox = PickedBox;

                // pick a meme
                Random pickRandomMeme = new();
                int PickedMeme = pickRandomMeme.Next(BTCSettings.Memes.Length);

                // write our chosen meme to Meme property
                CopiedPicData.Meme = BTCSettings.Memes[PickedMeme];

                // set the type of output
                CopiedPicData.OutputType = OutputType.Dankness;

                DankifyTarget.Varieties.Add(CopiedPicData);
            }
        }

        /// <summary>
        /// Creates Variety from an existing image, this will be on user interaction
        /// </summary>
        /// <param name="PictureInputData">The Image to create variety of</param>
        public static void ProduceSaturationVarietyData(PictureData PictureInputData)
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

                VarietyData.ParamForTextCreation.FillColor.SetByHSL(fillcolorHue, variety, fillcolorLuminance);

                VarietyData.ParamForTextCreation.StrokeColor.SetByHSL(strokecolorHue, variety, strokecolorLuminance);

                VarietyData.ParamForTextCreation.BorderColor.SetByHSL(bordercolorHue, variety, bordercolorLuminance);

                Bitmap src = new(VarietyData.FileName);

                VarietyData.ParamForTextCreation = CalculateBoxData(VarietyData.ParamForTextCreation.CurrentBox, src, VarietyData.ParamForTextCreation);

                VarietyData.OutputType = OutputType.SaturationVariety;
                PictureInputData.Varieties.Add(VarietyData);
            }

        }
    }

}