using BulkThumbnailCreator.DataMethods;

namespace BulkThumbnailCreator.Services
{
    public class LogoService
    {
        private List<string> Logos { get; set; }
        private Random random;
        public LogoService()
        {
            BuildLogos();
            random = new Random();
        }
        public string GetRandomLogo()
        {
            int chosenLogo = random.Next(Logos.Count);
            return Logos[chosenLogo];
        }
        private void BuildLogos()
        {
            Logos = LogoGeneration.GenerateHundredLogos();
        }
    }
}
