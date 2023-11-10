using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SECrawler.Entities;

namespace SECrawler.DataAccess.Configurations;

public class EnginesConfiguration : IEntityTypeConfiguration<Engine>
{
    public void Configure(EntityTypeBuilder<Engine> builder)
    {
        builder.ToTable("Engines");
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.Expression)
            .IsRequired();
        builder.Property(x => x.Name)
            .IsRequired();
        builder.Property(x => x.BaseUrl)
            .IsRequired();
        builder.Property(x => x.SearchUrl)
            .IsRequired();
    }
}