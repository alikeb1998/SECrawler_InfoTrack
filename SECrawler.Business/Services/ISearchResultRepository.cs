using SECrawler.Entities;

namespace SECrawler.Business.Services;

public interface ISearchResultRepository
{
    Task<List<SearchResult>> GetHistoriesAsync();

    Task AddResultAsync(List<int> ranks, string keywords, string url);
}