using Microsoft.AspNetCore.Mvc;
using SECrawler.Business.Services;

namespace SECrawler.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SearchResultController : Controller
{
    private readonly ISearchResultService _dataService;

    public SearchResultController(ISearchResultService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet(nameof(Histories))]
    public async Task<IActionResult> Histories()
    {
        var result = Ok(await _dataService.GetAllHistoriesAsync());
        return result;
    }
}