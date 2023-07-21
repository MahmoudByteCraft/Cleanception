using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Common.Models;

public abstract class BaseDetailsDto : IDto
{
    public DefaultIdType? ActivateBy { get; set; }
    public string? ActivateByName { get; set; }
    public DateTime? ActivateOn { get; set; }
    public bool IsActive { get; set; }

    public DefaultIdType CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }

    public DefaultIdType LastModifiedBy { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}