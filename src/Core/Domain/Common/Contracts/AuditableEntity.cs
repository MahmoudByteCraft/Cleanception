namespace Cleanception.Domain.Common.Contracts;

public abstract class AuditableEntity : AuditableEntity<DefaultIdType>
{
}

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity, ISoftDelete
{
    public DefaultIdType CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }

    public DefaultIdType LastModifiedBy { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public DateTime? DeletedOn { get; set; }
    public string? DeletedByName { get; set; }
    public DefaultIdType? DeletedBy { get; set; }

    protected AuditableEntity()
    {
        CreatedOn = DateTime.UtcNow;
        LastModifiedOn = DateTime.UtcNow;
    }
}
