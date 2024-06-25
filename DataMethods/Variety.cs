using BulkThumbnailCreator.Wrappers;

namespace BulkThumbnailCreator;

internal static class Variety
{
    /// <summary>
    /// Produces 5 varieties of the first randomized font that hasnt already been chosen
    /// </summary>
    /// <param name="pictureData">Your input PictureData object to produce varieties of</param>
    public static void Font(PictureData pictureData)
    {
        for (var box = 0; box < pictureData.BoxParameters.Count; box++)
        {
            if (pictureData.BoxParameters[box].CurrentBox.Type == BoxType.None)
            {
                break;
            }
            else
            {
                List<string> fontList = [pictureData.BoxParameters[box].Font];

                const int FontsToPick = 5;

                for (var i = 0; i < FontsToPick; i++)
                {
                    var directoryWrapper = new DirectoryWrapper();
                    var dg = new DataGeneration(directoryWrapper);
                    var pickedFont = dg.PickRandomFont();

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

                foreach (var font in fontList)
                {
                    PictureData createFontVariety = new(pictureData);
                    createFontVariety.Varieties.Clear();

                    createFontVariety.BoxParameters[box].Font = font;
                    createFontVariety.OutPutType = OutputType.FontVariety;
                    pictureData.Varieties.Add(createFontVariety);
                }
            }
        }
    }

    public static void Random(PictureData pictureInputData)
    {
        const int NumberOfRandomsToProduce = 5;

        for (var currentIndex = 0; currentIndex < NumberOfRandomsToProduce; currentIndex++)
        {
            for (var currentBoxes = 0; currentBoxes < pictureInputData._numberOfBoxes; currentBoxes++)
            {
                PictureData varietyData = new(pictureInputData);
                varietyData.Varieties.Clear();
                varietyData.BoxParameters[currentBoxes] = ColorData.SelectTwoDifferentColors(varietyData.BoxParameters[currentBoxes]);

                var directoryWrapper = new DirectoryWrapper();
                var dg = new DataGeneration(directoryWrapper);
                var font = dg.PickRandomFont();
                varietyData.BoxParameters[currentBoxes].Gradient = DataGeneration.RandomGradient();
                varietyData.BoxParameters[currentBoxes].Font = font;
                varietyData.OutPutType = OutputType.RandomVariety;
                pictureInputData.Varieties.Add(varietyData);
            }
        }
    }

    public static void Meme(PictureData pictureData)
    {
        // call the copy ctor on the picdata object
        PictureData copiedData = new(pictureData);

        // clear the variety list
        copiedData.Varieties.Clear();

        /// make a list that collects the boxes of the current picdata object
        /// this is so we can determine which boxes are available to pick from
        List<BoxType> readBoxes = [];

        for (var i = 0; i < copiedData.BoxParameters.Count; i++)
        {
            readBoxes.Add(copiedData.BoxParameters[i].CurrentBox.Type);
        }

        // we will now determine if a meme is appropriate
        // it should only be considered appropriate if there is atleast
        // one additional box besides the meme box
        // if that is not the case, memes shouldnt be used
        // so if readboxes count is less then 2, skip this iteration

        if (readBoxes.Count == 2 || readBoxes.Count > 2)
        {
            for (var i = 0; i < readBoxes.Count; i++)
            {
                // if we have looped and already made a meme, we need to make sure the text box
                // also gets passed

                if (copiedData.OutPutType == OutputType.MemeVariety)
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
                    var pickedMeme = pickRandomMeme.Next(Settings.Memes.Length);

                    // write our chosen meme to Meme property
                    copiedData.BoxParameters[i].Meme = Settings.Memes[pickedMeme];
                    copiedData.OutPutType = OutputType.MemeVariety;
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
        pictureData.Varieties.Add(copiedData);
    }
}
