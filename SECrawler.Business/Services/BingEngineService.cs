using System.Text.RegularExpressions;
using System.Web;
using AutoMapper;
using SECrawler.Dtos;

namespace SECrawler.Business.Services;

public class BingEngineService:IEngineService
{
    private readonly IEngineRepository _dataService;
    private readonly ISearchResultRepository _resultDataService;
    

    public BingEngineService(IEngineRepository dataService, ISearchResultRepository searchResultDataService)
    {
        _dataService = dataService;
        _resultDataService = searchResultDataService;
       
    }

  

    public async Task<List<int>> GetRankingsAsync(string query, int engineId, int pageSize)
    {
        var engine = await _dataService.GetOneAsync(engineId);
        if (engine == null) return new List<int>();

        var searchUrl = engine.SearchUrl.Replace("#query#", HttpUtility.UrlEncode(query))
            .Replace("#pageSize#", pageSize.ToString());
        using var client = new HttpClient();
        
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
        

        var content = await client.GetStringAsync($"{engine.BaseUrl}/{searchUrl}");
        var response = HttpUtility.HtmlDecode(content);
        var links = RetrieveLinksFromResponse(response, engine.Expression);
        var ranks = (from link in links
            where link.Contains("www.infotrack.co.uk", StringComparison.OrdinalIgnoreCase)
            select links.IndexOf(link)).Distinct().ToList();
        if (ranks.Count > 0)
        {
            await _resultDataService.AddResultAsync(ranks, query, engine.BaseUrl);
        }

        return ranks;
    }

    private List<string> RetrieveLinksFromResponse(string responseBody, string regexToExtractLinks)
    {
        var matches = Regex.Matches(responseBody, $"{regexToExtractLinks}");
        return matches.Select(x => x.Value).ToList();
    }
}