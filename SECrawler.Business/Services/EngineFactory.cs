using AutoMapper;

namespace SECrawler.Business.Services;

public class EngineFactory : IEngineFactory
{
    private readonly IEngineRepository _dataService;
    private readonly ISearchResultRepository _resultDataService;
   

    public EngineFactory(IEngineRepository dataService, ISearchResultRepository searchResultDataService)
    {
        _dataService = dataService;
        _resultDataService = searchResultDataService;
        
    }

    public IEngineService CreateEngineService( int engineId)
    {
        switch (engineId)
        {
            case 1:
                return new GoogleEngineService(_dataService, _resultDataService);
            case 2:
                return new BingEngineService(_dataService, _resultDataService);
            default:
                throw new ArgumentException("Invalid engine type");
        }
    }
}