using AutoMapper;
using SECrawler.Entities;

namespace SECrawler.Infrastructure.Mapping;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Engine, Dtos.Engine>().ReverseMap();
        CreateMap<IEnumerable<Engine>, IEnumerable<Dtos.Engine>>().ReverseMap();
        CreateMap<SearchResult, Dtos.SearchResult>().ReverseMap();
        CreateMap<IEnumerable<SearchResult>, IEnumerable<Dtos.SearchResult>>().ReverseMap();
        
    }
}