using Microsoft.Graph;

namespace IntranetSearchBot.Interfaces
{
    public interface IGraphHelper
    {
        GraphServiceClient GetDelegatedServiceClient(string _token);
    }
}
