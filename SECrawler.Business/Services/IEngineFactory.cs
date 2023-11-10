namespace SECrawler.Business.Services;

public interface IEngineFactory
{
   public IEngineService CreateEngineService(int engineId);
}