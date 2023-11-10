using Microsoft.AspNetCore.Mvc;
using SECrawler.Business.Services;

namespace SECrawler.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EngineController : Controller
{
    private readonly IEngineListService _engineService;
     private readonly IEngineFactory _engineFactory;

    public EngineController(IEngineListService engineService, IEngineFactory engineFactory)
    {
        _engineService = engineService;
         _engineFactory = engineFactory;
    }

    [HttpGet(nameof(Search))]
    public async Task<IActionResult> Search(string query, int engineId, int pageSize)
    {
        
        var service = _engineFactory.CreateEngineService( engineId);
        var result = Ok( await  service.GetRankingsAsync(query,engineId, pageSize));
        return result;
    }

    [HttpGet(nameof(SearchEngines))]
    public async Task<IActionResult> SearchEngines()
    {
        var result = Ok(await _engineService.GetEnginesAsync());
        return result;
    }
}