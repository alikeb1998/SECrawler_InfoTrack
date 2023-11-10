namespace SECrawler.Entities;

public class SearchResult
{
    public long Id { get; set; }
    public string Url { get; set; }

    public string KeyWords { get; set; }

    public string Rank { get; set; }

    public DateTime Date { get; set; }
}