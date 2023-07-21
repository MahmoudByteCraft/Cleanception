using System.Text.Json.Serialization;

namespace Cleanception.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContinentType
{
    Asia,
    Africa,
    NorthAmerica,
    SouthAmerica,
    Antarctica,
    Europe,
    Australia
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityToChange
{
    Country,
    State,
    City
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OperationType
{
    SetAsActive,
    SetAsInActive,
    SetAsDeleted
}