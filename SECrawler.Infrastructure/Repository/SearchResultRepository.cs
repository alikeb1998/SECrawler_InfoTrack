using Microsoft.EntityFrameworkCore;
using SECrawler.Business.Services;
using SECrawler.DataAccess;
using SECrawler.Entities;

namespace SECrawler.Infrastructure.Repository;

public class SearchResultRepository: ISearchResultRepository
{
    private readonly EfDbContext _dbContext;

    public SearchResultRepository(EfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SearchResult>> GetHistoriesAsync()
    {
        return await _dbContext.SearchResults.ToListAsync();
      
    }

    public async Task AddResultAsync(List<int> rankings, string keywords, string url)
    {
        var result = new SearchResult()
        {
            Url = url,
            KeyWords = keywords,
            Date = DateTime.Now,
            Rank = string.Join(",", rankings.Select(x => x.ToString())),
        };

        await _dbContext.SearchResults.AddAsync(result);
        await _dbContext.SaveChangesAsync();
    }
}