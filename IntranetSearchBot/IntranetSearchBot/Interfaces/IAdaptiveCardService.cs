using AdaptiveCards;
using IntranetSearchBot.Models.Search;

namespace IntranetSearchBot.Interfaces
{
    public interface IAdaptiveCardService
    {
        List<AdaptiveElement> GetElements<T>(List<T> items);
        string BindData<T>(string adaptiveCard, T data);
        AdaptiveElement ConvertJsonToAdaptiveElement(string content);
        List<AdaptiveElement> GetSearchResultsContainers(SearchResults results);
    }
}
