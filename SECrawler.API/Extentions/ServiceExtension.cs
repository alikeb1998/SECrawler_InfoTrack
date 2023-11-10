using Microsoft.EntityFrameworkCore;
using SECrawler.Business.Services;
using SECrawler.DataAccess;
using SECrawler.Infrastructure.Repository;
using Serilog;

namespace SECrawler.API.Extensions;

public static class ServiceExtension
{
    public static void Inject(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSerilog();
        var connectionString = configuration.GetConnectionString("sqlConnection");
        services.AddDbContext<EfDbContext>(options => { options.UseSqlServer(connectionString); });

        services.AddScoped<IEngineRepository, EngineRepository>();
        services.AddScoped<ISearchResultRepository, SearchResultRepository>();
        
        services.AddScoped<IEngineFactory, EngineFactory>();
        
        services.AddTransient<IEngineListService, EngineService>();
        services.AddTransient<ISearchResultService, SearchResultService>();
        
        services.AddScoped<IEngineService, GoogleEngineService>();
        services.AddScoped<IEngineService, BingEngineService>();
    }
}