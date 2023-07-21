namespace Cleanception.Application.Common.Shared;

public interface IActiveRequest<TId>
{
    public TId Id { get; set; }
    public bool IsActive { get; set; }
}
