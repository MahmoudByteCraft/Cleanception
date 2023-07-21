using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Identity.Users;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}