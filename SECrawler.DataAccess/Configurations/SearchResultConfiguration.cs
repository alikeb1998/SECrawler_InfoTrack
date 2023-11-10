using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SECrawler.Entities;

namespace SECrawler.DataAccess.Configurations;

public class SearchResultConfiguration: IEntityTypeConfiguration<SearchResult>
{
    public void Configure(EntityTypeBuilder<SearchResult> builder)
    {
        builder.ToTable("SearchResults");
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.Rank)
            .IsRequired();
        builder.Property(x => x.Url)
            .IsRequired();
        builder.Property(x => x.KeyWords)
            .IsRequired();
    }
}