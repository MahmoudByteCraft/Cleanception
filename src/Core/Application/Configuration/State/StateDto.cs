using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Configuration.Country;

namespace Cleanception.Application.Configuration.State;

public class StateSimplifyDto : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? DialCode { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }
}

public class StateRawDto : StateSimplifyDto
{
    public Guid CountryId { get; set; }
}

public class StateDto : StateRawDto
{
    public CountryRawDto? Country { get; set; }
}

public class StateDetailsDto : BaseDetailsDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public Guid CountryId { get; set; }
    public CountryRawDto? Country { get; set; }
}