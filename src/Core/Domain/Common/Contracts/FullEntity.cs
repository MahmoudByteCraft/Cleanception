namespace Cleanception.Domain.Common.Contracts;

public abstract class FullEntity : FullEntity<DefaultIdType>
{
}

public abstract class FullEntity<T> : AuditableEntity<T>, IHaveActivation
{
    public bool IsActive { get; set; }
    public DateTime? ActivationChangedOn { get; set; }
    public string? ActivationChangedByName { get; set; }
    public DefaultIdType ActivationChangedBy { get; set; }

    protected FullEntity()
    {
    }
}