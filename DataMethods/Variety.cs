namespace BulkThumbnailCreator;

public class Variety
{
    private readonly IDirectoryWrapper _directoryWrapper;
    private readonly ISettings _isettings;

    public Variety(IDirectoryWrapper directoryWrapper, ISettings settings)
    {
        _directoryWrapper = directoryWrapper;
        _isettings = settings;
    }

    public void Random(PictureData pictureInputData)
    {
        const int NumberOfRandomsToProduce = 5;

        for (var currentIndex = 0; currentIndex < NumberOfRandomsToProduce; currentIndex++)
        {
            // create a copy of the object to varietize
            PictureData varietyData = new(pictureInputData);

            // clear the objects list of varieties.
            varietyData.Varieties.Clear();

            // create the values for each box parameter
            foreach (var boxparameter in varietyData.BoxParameters)
            {
                ColorData.SelectTwoRandomColors(boxparameter);
                var dg = new DataGeneration(_directoryWrapper);
                boxparameter.Gradient = DataGeneration.RandomBool();
                boxparameter.Shadows = DataGeneration.RandomBool();

                var excludeFonts = new List<string>();
                foreach (var liftfonts in varietyData.BoxParameters)
                {
                    excludeFonts.Add(liftfonts.Font);
                }
                do
                {
                    boxparameter.Font = dg.PickRandomFont();
                } while (excludeFonts.Contains(boxparameter.Font));

                varietyData.OutPutType = OutputType.RandomVariety;
            }
            pictureInputData.Varieties.Add(varietyData);
        }
    }

    public void Meme(PictureData pictureData)
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

                if (copiedData.OutPutType is not OutputType.MemeVariety)
                {
                    // pick a meme
                    Random pickRandomMeme = new();
                    var pickedMeme = pickRandomMeme.Next(_isettings.Memes.Length);

                    // write our chosen meme to Meme property
                    copiedData.BoxParameters[i].Meme = _isettings.Memes[pickedMeme];
                    copiedData.OutPutType = OutputType.MemeVariety;
                }
            }
        }
        // do not modify picdata object
        // do not write any data to lists

        // add the copied data to the list of varieties if all is successful
        pictureData.Varieties.Add(copiedData);
    }
}
