using AutoMapper;
using SECrawler.Dtos;

namespace SECrawler.Business.Services;

public class SearchResultService : ISearchResultService
{
    private readonly ISearchResultRepository _dataService;
    // private readonly IMapper _mapper;

    public SearchResultService(ISearchResultRepository dataService/*, IMapper mapper*/)
    {
        _dataService = dataService;
        // _mapper = mapper;
    }

    public async Task<IEnumerable<SearchResult>> GetAllHistoriesAsync()
    {
         var result = await _dataService.GetHistoriesAsync();
         return result.Select(x => new SearchResult()
         {
             Id = x.Id,
             Date = x.Date,
             Rank = x.Rank,
             Url = x.Url,
             KeyWords = x.KeyWords
         });
    }
}