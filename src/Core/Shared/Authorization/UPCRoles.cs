using System.Collections.ObjectModel;

namespace Cleanception.Shared.Authorization;

public static class UPCRoles
{
    public const string Admin = nameof(Admin);
    public const string Customer = nameof(Customer);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Customer,Admin
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}