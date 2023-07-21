using Microsoft.Extensions.DependencyInjection;

namespace Cleanception.Infrastructure.Persistence.Initialization;

internal class CustomSeederRunner
{
    private readonly ICustomSeeder[] _seeders;

    public CustomSeederRunner(IServiceProvider serviceProvider) =>
        _seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders.OrderBy(x => x.Order()))
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}