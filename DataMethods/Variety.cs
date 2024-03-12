namespace BulkThumbnailCreator;

internal class Variety
{
    /// <summary>
    /// Produces 5 varieties of the first randomized font that hasnt already been chosen
    /// </summary>
    /// <param name="PicToVarietize">Your input PictureData object to produce varieties of</param>
    /// <param name="TargetFolder">The target folder of your object</param>
    public static void Font(PictureData PicToVarietize)
    {
        for (int Box = 0; Box < PicToVarietize.BoxParameters.Count; Box++)
        {
            if (PicToVarietize.BoxParameters[Box].CurrentBox.Type == BoxType.None)
            {
                break;
            }
            else
            {
                List<string> fontList = [PicToVarietize.BoxParameters[Box].Font];

                int FontsToPick = 5;

                for (int i = 0; i < FontsToPick; i++)
                {
                    string pickedFont = DataGeneration.PickRandomFont();

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
    /// 
    /// </summary>
    /// <param name="PictureInputData"></param>
    public static void Random(PictureData PictureInputData)
    {
        const int NumberOfRandomsToProduce = 15;

        for (int CurrentIndex = 0; CurrentIndex < NumberOfRandomsToProduce; CurrentIndex++)
        {
            for (int CurrentBoxes = 0; CurrentBoxes < PictureInputData.NumberOfBoxes; CurrentBoxes++)
            {
                PictureData VarietyData = new(PictureInputData);
                VarietyData.Varieties.Clear();
                VarietyData.BoxParameters[CurrentBoxes] = ColorData.SelectTwoDifferentColors(VarietyData.BoxParameters[CurrentBoxes]);
                string Font = DataGeneration.PickRandomFont();
                VarietyData.BoxParameters[CurrentBoxes].Font = Font;
                VarietyData.OutPutType = OutputType.RandomVariety;
                PictureInputData.Varieties.Add(VarietyData);
            }
        }
    }

    public static void Meme(PictureData picDataToVar)
    {
        // call the copy ctor on the picdata object
        PictureData CopiedData = new(picDataToVar);

        // clear the variety list
        CopiedData.Varieties.Clear();

        /// make a list that collects the boxes of the current picdata object
        /// this is so we can determine which boxes are available to pick from
        List<BoxType> ReadBoxes = [];

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
