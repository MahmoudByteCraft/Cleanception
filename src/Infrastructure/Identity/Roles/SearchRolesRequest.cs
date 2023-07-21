using Ardalis.Specification.EntityFrameworkCore;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Identity.Roles;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cleanception.Infrastructure.Identity.Roles;

public class SearchRolesRequest : PaginationFilter, IQuery<PaginatedResult<RoleDto>>
{
}

public class RolesBySearchRequestSpec : EntitiesByPaginationFilterSpec<ApplicationRole, RoleDto>
{
    public RolesBySearchRequestSpec(PaginationFilter filter) : base(filter)
    {
    }
}

public class SearchRolesRequestHandler : IQueryHandler<SearchRolesRequest, PaginatedResult<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public SearchRolesRequestHandler(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<PaginatedResult<RoleDto>> Handle(SearchRolesRequest request, CancellationToken cancellationToken)
    {
        var spec = new RolesBySearchRequestSpec(request);

        var query = SpecificationEvaluator.Default.GetQuery(_roleManager.Roles, spec, false).ProjectToType<RoleDto>();

        var list = await query.ToListAsync(cancellationToken);
        int count = await query.CountAsync(cancellationToken);
        return PaginatedResult<RoleDto>.Success(list, count, request.PageNumber, request.PageSize);
    }
}