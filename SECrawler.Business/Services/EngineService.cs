using System.Text.RegularExpressions;
using System.Web;
using SECrawler.Dtos;
using AutoMapper;

namespace SECrawler.Business.Services;

public class EngineService:IEngineListService
{
    private readonly IEngineRepository _dataService;
    private readonly IMapper _mapper;

    public EngineService(IEngineRepository dataService, IMapper mapper)
    {
        _dataService = dataService;
       
        _mapper = mapper;
    }

    public async Task<IEnumerable<Engine>> GetEnginesAsync()
    {
        var result = await _dataService.GetEnginesAsync();
        // var result = _mapper.Map<IEnumerable<Entities.Engine>>(result);
       
        return result.Select(x => new Engine()
        {
            Id = x.Id,
            SearchUrl = x.SearchUrl,
            Expression = x.Expression,
            BaseUrl = x.BaseUrl,
            Name = x.Name
        });
    }

}