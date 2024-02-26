namespace BulkThumbnailCreator;

public class DataGeneration
{
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
    public static ParamForTextCreation GetTextPosition(ParamForTextCreation parameters, Array2D<RgbPixel> sourcePicture, Rectangle[] faceRect, List<BoxType> populatedBoxes, PictureData pictureData = null)
    {
        List<Box> Boxes = [];
        Boxes = BuildDefaultBoxes(Boxes, sourcePicture);
        parameters.Boxes = Boxes;
        // boxes that we will pick from after calcs done

        List<BoxType> FreeBoxes = new();
        foreach (Box box in Boxes)
        {
            FreeBoxes.Add(box.Type);
        }

        for (int i = 0; i < populatedBoxes.Count; i++)
        {
            if (populatedBoxes[i] != BoxType.BottomLeft)
            {
                FreeBoxes.Remove(BoxType.BottomBox);
            }
            if (populatedBoxes[i] != BoxType.BottomRight)
            {
                FreeBoxes.Remove(BoxType.BottomBox);
            }
            if (populatedBoxes[i] == BoxType.BottomBox)
            {
                FreeBoxes.Remove(BoxType.BottomLeft);
                FreeBoxes.Remove(BoxType.BottomRight);
            }
            if (populatedBoxes[i] == BoxType.TopLeft)
            {
                FreeBoxes.Remove(BoxType.TopBox);
            }
            if (populatedBoxes[i] == BoxType.TopRight)
            {
                FreeBoxes.Remove(BoxType.TopBox);
            }
            if (populatedBoxes[i] == BoxType.TopBox)
            {
                FreeBoxes.Remove(BoxType.TopLeft);
                FreeBoxes.Remove(BoxType.TopRight);
            }
            if (populatedBoxes[i] == BoxType.BottomBox)
            {
                FreeBoxes.Remove(BoxType.BottomLeft);
                FreeBoxes.Remove(BoxType.BottomRight);
            }
        }

        List<(BoxType, Rectangle)> BoxRectangles = new();
        foreach (Box box in Boxes)
        {
            Rectangle boxRectangle = new Rectangle(box.X, box.Y, box.X + box.Width, box.Y + box.Height);
            BoxRectangles.Add((box.Type, boxRectangle));
        }

        if (faceRect.Length != 0)
        {
            foreach (var face in faceRect)
            {
                List<(BoxType, bool)> FaceInterSectResults = [];

                for (int i = 0; i < BoxRectangles.Count; i++)
                {
                    bool BoxIntersect;
                    BoxIntersect = IntersectCheck(BoxRectangles[i].Item2, face);

                    if (BoxIntersect)
                    {
                        FreeBoxes.Remove(BoxRectangles[i].Item1);
                    }
                    FaceInterSectResults.Add((BoxRectangles[i].Item1, BoxIntersect));
                }
            }
        }

        // boxes to delete from the list of free boxes
        List<BoxType> BoxesToDelete = [];

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
            //Boxes.TryGetValue(boxtype, out Rectangle BoxToCheckIntersect);
            // Use LINQ to find the tuple with matching box type and return its associated rectangle
            var tuple = BoxRectangles.FirstOrDefault(item => item.Item1 == boxtype);

            Rectangle rectangle = tuple.Item2;

            List<(BoxType, bool)> IntersectResults = [];

            for (int rectangles = 0; rectangles < BoxRectangles.Count; rectangles++)
            {
                //Boxes.TryGetValue(boxType, out Rectangle Box);

                bool BoxIntersect = IntersectCheck(rectangle, BoxRectangles[rectangles].Item2);

                if (BoxIntersect)
                {
                    FreeBoxes.Remove(BoxRectangles[rectangles].Item1);
                }
                IntersectResults.Add((BoxRectangles[rectangles].Item1, BoxIntersect));
            }
        }

        // check what boxes are left
        if (FreeBoxes.Count != 0)
        {
            // all calculations done, pick one box if there are any left
            Random random = new();
            BoxType pickedBoxName;
            BoxType[] boxes = [.. FreeBoxes];

            pickedBoxName = boxes[random.Next(boxes.Length)];

            parameters.CurrentBox = parameters.Boxes.Find(x => x.Type == pickedBoxName);
            parameters.WidthOfBox = parameters.CurrentBox.Width;
            parameters.HeightOfBox = parameters.CurrentBox.Height;

            FreeBoxes.Remove(pickedBoxName);

            parameters.BoxesWithNoFaceIntersect = FreeBoxes;
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
    private static List<Box> BuildDefaultBoxes(List<Box> Boxes, Array2D<RgbPixel> sourcePicture)
    {
        // top box
        Box topBox = new()
        {
            Type = BoxType.TopBox,
            Width = sourcePicture.Columns,
            Height = sourcePicture.Rows / 2,
            X = 0,
            Y = 0
        };
        Boxes.Add(topBox);
        //////////////////////////////////////////////

        //// bottom box
        Box bottomBox = new();
        bottomBox.Type = BoxType.BottomBox;
        bottomBox.Width = sourcePicture.Columns;
        bottomBox.Height = sourcePicture.Rows / 2;
        bottomBox.X = 0;
        bottomBox.Y = sourcePicture.Rows / 2;
        Boxes.Add(bottomBox);

        //Rectangle bottomBoxRectangle = new(0, sourcePicture.Rows / 2, width, sourcePicture.Rows);


        // top left box
        //BoxType topLeftBox = BoxType.TopLeft;
        Box topLeftBox = new();
        topLeftBox.Type = BoxType.TopLeft;
        topLeftBox.Width = sourcePicture.Columns / 2;
        topLeftBox.Height = sourcePicture.Rows / 2;
        topLeftBox.X = 0;
        topLeftBox.Y = 0;
        Boxes.Add(topLeftBox);
        //Rectangle topLeftBoxRectangle = new(0, 0, width, height);

        //////////////////////////////////////////////

        // top right box
        Box topRightBox = new();
        topRightBox.Type = BoxType.TopRight;
        topRightBox.Width = sourcePicture.Columns / 2;
        topRightBox.Height = sourcePicture.Rows / 2;
        topRightBox.X = sourcePicture.Columns / 2;
        topRightBox.Y = 0;
        Boxes.Add(topRightBox);

        //BoxType topRightBox = BoxType.TopRight;

        //Rectangle topRightBoxRectangle = new(sourcePicture.Columns / 2, 0, sourcePicture.Columns, height);

        //////////////////////////////////////////////

        // bottom left box
        Box bottomLeftBox = new();
        bottomLeftBox.Type = BoxType.BottomLeft;
        bottomLeftBox.Width = sourcePicture.Columns / 2;
        bottomLeftBox.Height = sourcePicture.Rows / 2;
        bottomLeftBox.X = 0;
        bottomLeftBox.Y = sourcePicture.Rows / 2;
        Boxes.Add(bottomLeftBox);

        //Boxes.Add(bottomLeftBox, bottomleftBoxRectangle);
        //////////////////////////////////////////////

        // bottom right box
        Box bottomRightBox = new();
        bottomRightBox.Type = BoxType.BottomRight;
        bottomRightBox.Width = sourcePicture.Columns / 2;
        bottomRightBox.Height = sourcePicture.Rows / 2;
        bottomRightBox.X = sourcePicture.Columns / 2;
        bottomRightBox.Y = sourcePicture.Rows / 2;
        Boxes.Add(bottomRightBox);
        //////////////////////////////////////////////

        return Boxes;
    }
}