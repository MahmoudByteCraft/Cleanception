using System.Reflection;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Domain.Configuration;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Infrastructure.Persistence.Initialization;
using Cleanception.Infrastructure.Seeds.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cleanception.Infrastructure.Seeds;

public class StatesSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<StatesSeeder> _logger;

    public StatesSeeder(ISerializerService serializerService, ILogger<StatesSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public int Order()
    {
        return SeedsOrder.States;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!_db.States.Any())
        {
            _logger.LogInformation("Started to Seed States.");
            string statesSource = await File.ReadAllTextAsync(path + "/Seeds/Resources/states.json", cancellationToken);

            var usa = await _db.Countries.FirstOrDefaultAsync(x => x.Code == "US", cancellationToken: cancellationToken);
            var states = _serializerService.Deserialize<List<State>>(statesSource);

            if (states != null)
            {
                foreach (var state in states)
                {
                    foreach(string cityName in state.CitiesNames)
                    {
                        state.Cities.Add(new City()
                        {
                            Name = cityName,
                        });
                    }

                    state.CountryId = usa!.Id;
                    await _db.States.AddAsync(state, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded States.");
        }
    }
}