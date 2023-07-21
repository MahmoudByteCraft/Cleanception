namespace Cleanception.Application.Identity.Tokens;

public class DetailedParams
{
    public string? PriceCode { get; set; }
    public bool CanOrder { get; set; }
    public List<string>? Roles { get; set; }
}

public class NewDetailedParams
{
    public List<string>? Roles { get; set; }
}