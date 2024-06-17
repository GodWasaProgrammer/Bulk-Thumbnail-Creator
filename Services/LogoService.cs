using BulkThumbnailCreator.DataMethods;

namespace BulkThumbnailCreator.Services
{
    public class LogoService
    {
        private List<string> Logos { get; set; }
        private readonly Random _random;
        public LogoService()
        {
            BuildLogos();
            _random = new Random();
        }
        public string GetRandomLogo()
        {
            var chosenLogo = _random.Next(Logos.Count);
            return Logos[chosenLogo];
        }
        private void BuildLogos()
        {
            Logos = LogoGeneration.GenerateHundredLogos();
        }
    }
}
