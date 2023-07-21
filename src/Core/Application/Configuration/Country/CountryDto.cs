using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Configuration.Country;

public class CountrySimplifyDto : IDto
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? DialCode { get; set; }
    public string? Code { get; set; }
    public string? Flag { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}

public class CountryRawDto : CountrySimplifyDto
{

}

public class CountryDto : CountryRawDto
{

}

public class CountryDetailsDto : BaseDetailsDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DialCode { get; set; }
    public string? Code { get; set; }
    public string? Flag { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

}