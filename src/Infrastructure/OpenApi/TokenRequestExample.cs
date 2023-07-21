using Cleanception.Application.Identity.Tokens;
using NSwag.Examples;

namespace Cleanception.Infrastructure.OpenApi;

public class TokenRequestExample : IExampleProvider<TokenRequest>
{
    public TokenRequest GetExample()
    {
        return new TokenRequest("fatcat@email.com", "123456789", null);
    }
}