using Cleanception.Domain.Attributes;
using Cleanception.Domain.Common.Contracts;

namespace Cleanception.Domain.Configuration;

[SupportDeepSearch]
public class City : FullEntity, IAggregateRoot
{

    [ColumnSupportDeepSearch]
    public string? Name { get; set; }

    [ColumnSupportDeepSearch]
    public string? Code { get; set; }

    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public Guid StateId { get; set; }

    [ColumnSupportDeepSearch]
    public virtual State State { get; private set; } = default!;
}
