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
        Dictionary<BoxType, Rectangle> Boxes = [];

        Boxes = BuildDefaultBoxes(Boxes, sourcePicture);

        foreach (var box in Boxes)
        {
            Box PassBox = new();
            Boxes.TryGetValue(box.Key, out Rectangle Box);
            PassBox.Width = (int)Box.Width;
            PassBox.Height = (int)Box.Height;
            PassBox.X = Box.Left;
            PassBox.Y = Box.Top;
            PassBox.Type = box.Key;
            parameters.Boxes.Add(PassBox);
        }

        // boxes that we will pick from after calcs done
        List<BoxType> FreeBoxes = [.. Boxes.Keys];

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

        if (faceRect.Length != 0)
        {
            foreach (var face in faceRect)
            {
                List<(BoxType, bool)> FaceInterSectResults = [];

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
            Boxes.TryGetValue(boxtype, out Rectangle BoxToCheckIntersect);
            List<(BoxType, bool)> IntersectResults = [];

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
    private static Dictionary<BoxType, Rectangle> BuildDefaultBoxes(Dictionary<BoxType, Rectangle> Boxes, Array2D<RgbPixel> sourcePicture)
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

        Rectangle bottomBoxRectangle = new(0, sourcePicture.Rows / 2, width, sourcePicture.Rows);

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
}