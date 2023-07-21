using System.Reflection;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Domain.Configuration;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Infrastructure.Persistence.Initialization;
using Cleanception.Infrastructure.Seeds.Resources;
using Microsoft.Extensions.Logging;

namespace Cleanception.Infrastructure.Seeds;

public class CountriesSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CountriesSeeder> _logger;

    public CountriesSeeder(ISerializerService serializerService, ILogger<CountriesSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public int Order()
    {
        return SeedsOrder.Countries;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!_db.Countries.Any())
        {
            _logger.LogInformation("Started to Seed Countries.");
            string countriesSource = await File.ReadAllTextAsync(path + "/Seeds/Resources/countries.json", cancellationToken);

            var countries = _serializerService.Deserialize<List<Country>>(countriesSource);

            if (countries != null)
            {
                foreach (var country in countries)
                {
                    await _db.Countries.AddAsync(country, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Countries.");
        }
    }
}