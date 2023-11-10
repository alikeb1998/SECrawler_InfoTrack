using Microsoft.EntityFrameworkCore;
using SECrawler.DataAccess;
using SECrawler.Entities;
using SECrawler.Business.Services;
namespace SECrawler.Infrastructure.Repository;

public class EngineRepository:IEngineRepository
{
    private readonly EfDbContext _dbContext;

    public EngineRepository(EfDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Engine>> GetEnginesAsync()
    {
        var searchEngines =  await _dbContext.Engines.ToListAsync();
        return searchEngines;
    }

    public async Task<Engine?> GetOneAsync(int id)
    {
        var searchEngine = await _dbContext.Engines.FirstOrDefaultAsync(x=>x.Id==id);
        return searchEngine;
    }
}