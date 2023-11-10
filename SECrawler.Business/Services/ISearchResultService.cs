using SECrawler.Dtos;

namespace SECrawler.Business.Services;

public interface ISearchResultService
{
    Task<IEnumerable<SearchResult>> GetAllHistoriesAsync();
}