using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Common.SMS;

public interface ISmsService : ITransientService
{
    Task SendAsync(string phonNumber, string message, CancellationToken ct = default);
    Task<bool> SendMulticast(string message, List<string?>? phoneNumbers, CancellationToken cancellationToken = default);
}