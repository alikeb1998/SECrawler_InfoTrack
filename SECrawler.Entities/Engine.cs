namespace SECrawler.Entities;

public class Engine
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string BaseUrl { get; set; }

    public string SearchUrl { get; set; }

    public string Expression { get; set; }
}