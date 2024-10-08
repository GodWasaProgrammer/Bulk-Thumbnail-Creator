﻿namespace BulkThumbnailCreator.DataMethods;

public class Variety
{
    private readonly IDirectoryWrapper _directoryWrapper;
    private readonly ISettings _isettings;

    public Variety(IDirectoryWrapper directoryWrapper, ISettings settings)
    {
        _directoryWrapper = directoryWrapper;
        _isettings = settings;
    }

    public async Task Fonts(PictureData pictureData)
    {
        // we will make 6 new font varieties
        const int FontVarietiesToMake = 6;

        for (var i = 0; i < FontVarietiesToMake; i++)
        {
            PictureData varietyData = new(pictureData);
            varietyData.Varieties.Clear();
            foreach (var boxparameter in varietyData.BoxParameters)
            {
                var dg = new DataGeneration(_directoryWrapper);
                var excludeFonts = boxparameter.Font;

                do
                {
                    boxparameter.Font = await Task.Run(() => dg.PickRandomFont());
                } while (excludeFonts.Contains(boxparameter.Font));
            }
            // add the result
            varietyData.OutPutType = OutputType.FontVariety;
            pictureData.Varieties.Add(varietyData);
        }
        return;
    }

    public static void Colors(PictureData pictureData)
    {
        const int VarietiesToMake = 6;

        for (var i = 0; i < VarietiesToMake; i++)
        {
            var varietyData = new PictureData(pictureData);
            varietyData.Varieties.Clear();
            foreach (var boxparam in varietyData.BoxParameters)
            {
                ColorData.SelectTwoRandomColors(boxparam);
            }
            varietyData.OutPutType = OutputType.ColorVariety;
            pictureData.Varieties.Add(varietyData);
        }
    }

    public static void Boxes(PictureData pictureData)
    {
        const int BoxesToMake = 6;

        var liftBoxes = pictureData.BoxParameters[0].Boxes;

        for (var i = 0; i < BoxesToMake; i++)
        {
            var varietyData = new PictureData(pictureData);
            varietyData.Varieties.Clear();

            foreach (var boxparam in varietyData.BoxParameters)
            {
                var boxtoWrite = BoxType.None;
                if (boxparam.CurrentBox.Type is BoxType.BottomBox)
                {
                    boxtoWrite = BoxType.TopBox;
                }
                if (boxparam.CurrentBox.Type is BoxType.TopBox)
                {
                    boxtoWrite = BoxType.BottomBox;
                }
                if (boxparam.CurrentBox.Type is BoxType.BottomLeft)
                {
                    boxtoWrite = BoxType.TopLeft;
                }
                if (boxparam.CurrentBox.Type is BoxType.BottomRight)
                {
                    boxtoWrite = BoxType.TopRight;
                }
                if (boxparam.CurrentBox.Type is BoxType.TopRight)
                {
                    boxtoWrite = BoxType.BottomRight;
                }
                if (boxparam.CurrentBox.Type is BoxType.TopLeft)
                {
                    boxtoWrite = BoxType.BottomLeft;
                }
                boxparam.CurrentBox = liftBoxes.Find(q => q.Type == boxtoWrite);
            }
            varietyData.OutPutType = OutputType.BoxVariety;
            pictureData.Varieties.Add(varietyData);
        }
    }

    public static void FX(PictureData pictureData)
    {
        const int FXToMake = 6;

        for (var i = 0; i < FXToMake; i++)
        {
            var varietyData = new PictureData(pictureData);
            varietyData.Varieties.Clear();
            foreach (var boxparam in varietyData.BoxParameters)
            {
                boxparam.Gradient = DataGeneration.RandomBool();
                boxparam.Shadows = DataGeneration.RandomBool();
            }
            varietyData.OutPutType = OutputType.FXVariety;
            pictureData.Varieties.Add(varietyData);
        }
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
