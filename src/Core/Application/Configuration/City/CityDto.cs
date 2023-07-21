using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Configuration.State;

namespace Cleanception.Application.Configuration.City;

public class CitySimplifyDto : IDto
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}

public class CityRawDto : CitySimplifyDto
{
    public Guid StateId { get; set; }
}

public class CityDto : CityRawDto
{
    public StateRawDto? State { get; set; }
}

public class CityDetailsDto : BaseDetailsDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public Guid StateId { get; set; }
    public StateRawDto? State { get; set; }
}

public class CityFullRelationDto : CityRawDto
{
    public StateDto? State { get; set; }
}