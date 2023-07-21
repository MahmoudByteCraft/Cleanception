using Cleanception.Domain.Attributes;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Domain.Enums;

namespace Cleanception.Domain.Configuration;

[SupportDeepSearch]
public class Country : FullEntity, IAggregateRoot
{
    public Country()
    {
        States = new HashSet<State>();
    }

    [ColumnSupportDeepSearch]
    public string? Name { get; set; }

    [ColumnSupportDeepSearch]
    public string? Code { get; set; }

    [ColumnSupportDeepSearch]
    public string? DialCode { get; set; }

    public string? Flag { get; set; }

    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public ContinentType? ContinentType { get; set; }

    public ICollection<State> States { get; set; }
}
