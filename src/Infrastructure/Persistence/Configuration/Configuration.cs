using Cleanception.Domain.Configuration;
using Cleanception.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cleanception.Infrastructure.Persistence.Configuration;

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
        => builder.ToTable(nameof(ApplicationDbContext.Countries), "Configuration");
}

public class StateConfig : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
        => builder.ToTable(nameof(ApplicationDbContext.States), "Configuration");
}

public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
        => builder.ToTable(nameof(ApplicationDbContext.Cities), "Configuration");
}

