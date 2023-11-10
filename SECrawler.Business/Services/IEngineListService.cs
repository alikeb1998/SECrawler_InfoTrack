using SECrawler.Dtos;

namespace SECrawler.Business.Services;

public interface IEngineListService
{
    Task<IEnumerable<Engine>> GetEnginesAsync();
}