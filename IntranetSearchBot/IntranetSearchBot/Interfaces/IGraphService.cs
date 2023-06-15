using IntranetSearchBot.Models.Search;
using Microsoft.Graph;

namespace IntranetSearchBot.Interfaces
{
    public interface IGraphService
    {
        void SetAccessToken(string token);
        Task<User> GetCurrentUserInfo();
        Task<SearchResults> Search(EntityType entityTypes, string queryString, int from = 0);
    }
}
