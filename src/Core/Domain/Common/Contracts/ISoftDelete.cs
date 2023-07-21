namespace Cleanception.Domain.Common.Contracts;

public interface ISoftDelete
{
    DateTime? DeletedOn { get; set; }
    Guid? DeletedBy { get; set; }
    string? DeletedByName { get; set; }
}

public interface IHaveActivation
{
    public bool IsActive { get; set; }
    public DateTime? ActivationChangedOn { get; set; }
    public string? ActivationChangedByName { get; set; }
    public DefaultIdType ActivationChangedBy { get; set; }
}