namespace BulkThumbnailCreator;

public static class DataGeneration
{
    /// <summary>
    /// Picks a random font from the provided font folder
    /// </summary>
    /// <returns></returns>
    public static string PickRandomFont()
    {
        var fontNames = Directory.GetFiles("Fonts", "*.TTF*");

        Random random = new();

        var randommax = fontNames.Length;

        int fontChosen = random.Next(randommax);

        return fontNames[fontChosen].ToString();
    }
    public static ParamForTextCreation GetTextPosition(ParamForTextCreation parameters, Array2D<RgbPixel> sourcePicture, Rectangle[] faceRect, List<BoxType> populatedBoxes)
    {
        List<Box> Boxes = BuildDefaultBoxes(sourcePicture);
        parameters.Boxes = Boxes;
        // boxes that we will pick from after calcs done

        List<BoxType> freeBoxes = [];
        foreach (Box box in Boxes)
        {
            freeBoxes.Add(box.Type);
        }

        if (faceRect.Length != 0)
        {
            foreach (var face in faceRect)
            {
                List<(BoxType, bool)> FaceInterSectResults = [];
                for (int i = 0; i < Boxes.Count; i++)
                {
                    var boxIntersect = IntersectCheck(Boxes[i].Rectangle, face);
                    if (boxIntersect)
                    {
                        freeBoxes.Remove(Boxes[i].Type);
                    }
                    FaceInterSectResults.Add((Boxes[i].Type, boxIntersect));
                }
            }
        }

        // early exit if there are no free boxes left after intersecting with faces
        if (freeBoxes.Count == 0)
        {
            return parameters;
        }

        if (populatedBoxes.Count != 0)
        {
            for (int BoxIndex = 0; BoxIndex < populatedBoxes.Count; BoxIndex++)
            {
                BoxType boxtype = populatedBoxes[BoxIndex];
                Rectangle PopulatedRect = Boxes.Find(x => x.Type == boxtype).Rectangle;
                List<(BoxType, bool)> IntersectResults = [];
                for (int rectangleIndex = 0; rectangleIndex < Boxes.Count; rectangleIndex++)
                {
                    Rectangle BoxRectangle = Boxes[rectangleIndex].Rectangle;
                    bool BoxIntersect = IntersectCheck(PopulatedRect, BoxRectangle);
                    if (BoxIntersect)
                    {
                        freeBoxes.Remove(Boxes[rectangleIndex].Type);
                    }
                    IntersectResults.Add((Boxes[rectangleIndex].Type, BoxIntersect));
                }
            }
        }

        // check what boxes are left
        if (freeBoxes.Count != 0)
        {
            // all calculations done, pick one box
            Random random = new();
            BoxType pickedBoxName;
            BoxType[] boxes = [.. freeBoxes];

            pickedBoxName = boxes[random.Next(boxes.Length)];

            parameters.CurrentBox = parameters.Boxes.Find(x => x.Type == pickedBoxName);
            parameters.WidthOfBox = parameters.CurrentBox.Width;
            parameters.HeightOfBox = parameters.CurrentBox.Height;

            freeBoxes.Remove(pickedBoxName);

            parameters.BoxesWithNoFaceIntersect = freeBoxes;
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
    private static List<Box> BuildDefaultBoxes(Array2D<RgbPixel> sourcePicture)
    {
        List<Box> Boxes = [];
        // top box
        Box topBox = new()
        {
            Type = BoxType.TopBox,
            Width = sourcePicture.Columns,
            Height = sourcePicture.Rows / 2,
            X = 0,
            Y = 0,
        };
        topBox.Rectangle = new(topBox.X, topBox.Y, topBox.X + topBox.Width, topBox.Y + topBox.Height);
        Boxes.Add(topBox);
        //////////////////////////////////////////////

        //// bottom box
        Box bottomBox = new()
        {
            Type = BoxType.BottomBox,
            Width = sourcePicture.Columns,
            Height = sourcePicture.Rows / 2,
            X = 0,
            Y = sourcePicture.Rows / 2
        };
        bottomBox.Rectangle = new(bottomBox.X, bottomBox.Y, bottomBox.X + bottomBox.Width, bottomBox.Y + bottomBox.Height);
        Boxes.Add(bottomBox);

        // top left box
        Box topLeftBox = new()
        {
            Type = BoxType.TopLeft,
            Width = sourcePicture.Columns / 2,
            Height = sourcePicture.Rows / 2,
            X = 0,
            Y = 0
        };
        topLeftBox.Rectangle = new(topLeftBox.X, topLeftBox.Y, topLeftBox.X + topLeftBox.Width, topLeftBox.Y + topLeftBox.Height);
        Boxes.Add(topLeftBox);
        //////////////////////////////////////////////

        // top right box
        Box topRightBox = new()
        {
            Type = BoxType.TopRight,
            Width = sourcePicture.Columns / 2,
            Height = sourcePicture.Rows / 2,
            X = sourcePicture.Columns / 2,
            Y = 0
        };
        topRightBox.Rectangle = new(topRightBox.X, topRightBox.Y, topRightBox.X + topRightBox.Width, topRightBox.Y + topRightBox.Height);
        Boxes.Add(topRightBox);
        //////////////////////////////////////////////

        // bottom left box
        Box bottomLeftBox = new()
        {
            Type = BoxType.BottomLeft,
            Width = sourcePicture.Columns / 2,
            Height = sourcePicture.Rows / 2,
            X = 0,
            Y = sourcePicture.Rows / 2
        };
        bottomLeftBox.Rectangle = new(bottomLeftBox.X, bottomLeftBox.Y, bottomLeftBox.X + bottomLeftBox.Width, bottomLeftBox.Y + bottomLeftBox.Height);
        Boxes.Add(bottomLeftBox);
        //////////////////////////////////////////////

        // bottom right box
        Box bottomRightBox = new()
        {
            Type = BoxType.BottomRight,
            Width = sourcePicture.Columns / 2,
            Height = sourcePicture.Rows / 2,
            X = sourcePicture.Columns / 2,
            Y = sourcePicture.Rows / 2
        };
        bottomRightBox.Rectangle = new(bottomRightBox.X, bottomRightBox.Y, bottomRightBox.X + bottomRightBox.Width, bottomRightBox.Y + bottomRightBox.Height);
        Boxes.Add(bottomRightBox);
        //////////////////////////////////////////////

        return Boxes;
    }
}