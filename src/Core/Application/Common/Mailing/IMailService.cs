using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Common.Mailing;

public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}