namespace Cleanception.Infrastructure.SMS;

public class SmsSettings
{
    public bool Enabled { get; set; }
    public string? FromPhoneNumber { get; set; }
    public string? AccountSid { get; set; }
    public string? AuthToken { get; set; }
}