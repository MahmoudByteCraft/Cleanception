namespace Cleanception.Application.Common.Shared;

public interface IDeleteRequest<TId>
{
    public TId Id { get; set; }
}