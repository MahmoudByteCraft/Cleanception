namespace Cleanception.Application.Identity.Interfaces;
public interface IFirebaseIdentityUser
{
    public string? FcmToken { get; set; }
}
