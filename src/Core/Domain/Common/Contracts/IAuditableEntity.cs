namespace Cleanception.Domain.Common.Contracts;

public interface IAuditableEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; }
    public string? CreatedByName { get; set; }

    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedByName { get; set; }

}