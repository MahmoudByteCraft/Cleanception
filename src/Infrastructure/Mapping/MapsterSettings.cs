using Cleanception.Application.Identity.Users;
using Cleanception.Infrastructure.Identity;
using Mapster;

namespace Cleanception.Infrastructure.Mapping;

public class MapsterSettings
{
    public static TypeAdapterConfig Configure()
    {
        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping

        var config = new TypeAdapterConfig();

        //TypeAdapterConfig<ApplicationUser, UserDetailsDto>.NewConfig()
        //     .Map(dest => dest.Roles, src => src.ApplicationUserRoles.Select(x => x.Role));

        TypeAdapterConfig<ApplicationUserRole, RoleDto>.NewConfig().Map(dest => dest, src => src.Role);

        return config;
    }
}