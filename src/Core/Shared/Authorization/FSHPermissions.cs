using System.Collections.ObjectModel;

namespace Cleanception.Shared.Authorization;

public static class FSHAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
}

public static class FSHResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Countries = nameof(Countries);
    public const string States = nameof(States);
    public const string Cities = nameof(Cities);
}

public static class FSHPermissions
{
    private static readonly FSHPermission[] _all = new FSHPermission[]
    {
        new("View Users", FSHAction.View, FSHResource.Users),
        new("Search Users", FSHAction.Search, FSHResource.Users),
        new("Create Users", FSHAction.Create, FSHResource.Users),
        new("Update Users", FSHAction.Update, FSHResource.Users),
        new("Delete Users", FSHAction.Delete, FSHResource.Users),
        new("Export Users", FSHAction.Export, FSHResource.Users),

        new("View UserRoles", FSHAction.View, FSHResource.UserRoles),
        new("Update UserRoles", FSHAction.Update, FSHResource.UserRoles),

        new("View Roles", FSHAction.View, FSHResource.Roles),
        new("Create Roles", FSHAction.Create, FSHResource.Roles),
        new("Update Roles", FSHAction.Update, FSHResource.Roles),
        new("Delete Roles", FSHAction.Delete, FSHResource.Roles),

        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Update RoleClaims", FSHAction.Update, FSHResource.RoleClaims),

        new("View Countries", FSHAction.View, FSHResource.Countries, IsCustomer: true),
        new("Search Countries", FSHAction.Search, FSHResource.Countries, IsCustomer: true),
        new("Create Countries", FSHAction.Create, FSHResource.Countries),
        new("Update Countries", FSHAction.Update, FSHResource.Countries),
        new("Delete Countries", FSHAction.Delete, FSHResource.Countries),
        new("Export Countries", FSHAction.Export, FSHResource.Countries),

        new("View States", FSHAction.View, FSHResource.States, IsCustomer: true),
        new("Search States", FSHAction.Search, FSHResource.States, IsCustomer: true),
        new("Create States", FSHAction.Create, FSHResource.States),
        new("Update States", FSHAction.Update, FSHResource.States),
        new("Delete States", FSHAction.Delete, FSHResource.States),

        new("View Cities", FSHAction.View, FSHResource.Cities, IsCustomer: true),
        new("Search Cities", FSHAction.Search, FSHResource.Cities, IsCustomer: true),
        new("Create Cities", FSHAction.Create, FSHResource.Cities),
        new("Update Cities", FSHAction.Update, FSHResource.Cities),
        new("Delete Cities", FSHAction.Delete, FSHResource.Cities),
    };

    public static IReadOnlyList<FSHPermission> All { get; } = new ReadOnlyCollection<FSHPermission>(_all);
    public static IReadOnlyList<FSHPermission> Admin { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FSHPermission> Customer { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsCustomer).ToArray());
}

public record FSHPermission(string Description, string Action, string Resource, bool IsCustomer = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}