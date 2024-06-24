namespace BulkThumbnailCreator;

public class DataGeneration
{
    private readonly IDirectoryWrapper _directoryWrapper;

    public DataGeneration(IDirectoryWrapper directoryWrapper)
    {
        _directoryWrapper = directoryWrapper;
    }

    /// <summary>
    /// Picks a random font from the provided font folder
    /// </summary>
    /// <returns></returns>
    public string PickRandomFont()
    {
        var fontNames = _directoryWrapper.GetFiles("Fonts", "*.TTF*");

        var randommax = fontNames.Length;

        var fontChosen = new Random().Next(randommax);

        return fontNames[fontChosen];
    }
    public static ParamForTextCreation GetTextPosition(ParamForTextCreation parameters, Rectangle[] faceRect, List<BoxType> populatedBoxes)
    {
        var defaultBoxes = parameters.Boxes;
        // boxes that we will pick from after calcs done

        // checks to see if we have boxes not occupied by faces or other content
        var freeBoxes = CalculateFreeBoxes(faceRect, defaultBoxes);

        // early exit if there are no free boxes left after intersecting with faces
        if (freeBoxes.Count == 0)
        {
            return parameters;
        }

        // excludes our populated boxes from the calculations
        ExcludePopulatedBoxesIntersection(populatedBoxes, defaultBoxes, freeBoxes);

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

    private static void ExcludePopulatedBoxesIntersection(List<BoxType> populatedBoxes, List<Box> defaultBoxes, List<BoxType> freeBoxes)
    {
        if (populatedBoxes.Count != 0)
        {
            for (var boxIndex = 0; boxIndex < populatedBoxes.Count; boxIndex++)
            {
                var boxtype = populatedBoxes[boxIndex];
                var populatedRect = defaultBoxes.Find(x => x.Type == boxtype).Rectangle;
                List<(BoxType, bool)> intersectResults = [];
                for (var rectangleIndex = 0; rectangleIndex < defaultBoxes.Count; rectangleIndex++)
                {
                    var boxRectangle = defaultBoxes[rectangleIndex].Rectangle;
                    var boxIntersect = IntersectCheck(populatedRect, boxRectangle);
                    if (boxIntersect)
                    {
                        freeBoxes.Remove(defaultBoxes[rectangleIndex].Type);
                    }
                    intersectResults.Add((defaultBoxes[rectangleIndex].Type, boxIntersect));
                }
            }
        }
    }

    private static List<BoxType> CalculateFreeBoxes(Rectangle[] faceRect, List<Box> defaultBoxes)
    {
        List<BoxType> freeBoxes = [];
        foreach (var box in defaultBoxes)
        {
            freeBoxes.Add(box.Type);
        }

        if (faceRect.Length != 0)
        {
            foreach (var face in faceRect)
            {
                List<(BoxType, bool)> faceInterSectResults = [];
                for (var i = 0; i < defaultBoxes.Count; i++)
                {
                    var boxIntersect = IntersectCheck(defaultBoxes[i].Rectangle, face);
                    if (boxIntersect)
                    {
                        freeBoxes.Remove(defaultBoxes[i].Type);
                    }
                    faceInterSectResults.Add((defaultBoxes[i].Type, boxIntersect));
                }
            }
        }

        return freeBoxes;
    }

    public static bool IntersectCheck(Rectangle rect1, Rectangle rect2)
    {
        return rect1.Left < rect2.Right &&
               rect1.Right > rect2.Left &&
               rect1.Top < rect2.Bottom &&
               rect1.Bottom > rect2.Top;
    }
    public static List<Box> BuildDefaultBoxes(Array2D<RgbPixel> sourcePicture)
    {
        List<Box> boxes = [];
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
        boxes.Add(topBox);
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
        boxes.Add(bottomBox);

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
        boxes.Add(topLeftBox);
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
        boxes.Add(topRightBox);
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
        boxes.Add(bottomLeftBox);
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
        boxes.Add(bottomRightBox);
        //////////////////////////////////////////////

        return boxes;
    }
    public static bool RandomGradient()
    {
        var random = new Random();
        var randomBool = random.Next(2) == 0;
        return randomBool;
    }
}
