using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<List<AuditDto>> GetUserTrailsAsync(Guid userId);
}