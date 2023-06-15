using IntranetSearchBot.Interfaces;

namespace IntranetSearchBot.Services
{
    public class FileService: IFileService
    {
        public string GetCard(string cardName)
        {
            var filePath = Path.Combine(".", "Resources", $"{cardName}.json");
            var card = File.ReadAllText(filePath);
            return card;
        }
    }
}
