using SECrawler.Entities;
namespace SECrawler.Business.Services;

public interface IEngineRepository
{
    Task<IEnumerable<Engine>> GetEnginesAsync();

    Task<Engine?> GetOneAsync(int id);
}