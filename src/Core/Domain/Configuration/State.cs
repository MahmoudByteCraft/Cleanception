using System.ComponentModel.DataAnnotations.Schema;
using Cleanception.Domain.Attributes;
using Cleanception.Domain.Common.Contracts;

namespace Cleanception.Domain.Configuration;

[SupportDeepSearch]
public class State : FullEntity, IAggregateRoot
{
    public State()
    {
        Cities = new HashSet<City>();
    }

    [ColumnSupportDeepSearch]
    public string? Name { get; set; }

    [ColumnSupportDeepSearch]
    public string? Code { get; set; }

    [ColumnSupportDeepSearch]
    public string? DialCode { get; set; }

    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public Guid CountryId { get; set; }

    [ColumnSupportDeepSearch]
    public virtual Country Country { get; private set; } = default!;

    public ICollection<City> Cities { get; set; }
    [NotMapped]
    public ICollection<string> CitiesNames { get; set; } = default!;
}
