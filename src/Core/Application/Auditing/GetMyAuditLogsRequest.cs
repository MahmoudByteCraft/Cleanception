using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;

namespace Cleanception.Application.Auditing;

public class GetMyAuditLogsRequest : IQuery<List<AuditDto>>
{
}

public class GetMyAuditLogsRequestHandler : IQueryHandler<GetMyAuditLogsRequest, List<AuditDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuditService _auditService;

    public GetMyAuditLogsRequestHandler(ICurrentUser currentUser, IAuditService auditService) =>
        (_currentUser, _auditService) = (currentUser, auditService);

    public Task<List<AuditDto>> Handle(GetMyAuditLogsRequest request, CancellationToken cancellationToken) =>
        _auditService.GetUserTrailsAsync(_currentUser.GetUserId());
}