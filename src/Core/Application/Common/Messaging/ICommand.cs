using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Common.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : ICommand<Result<Guid>>
{
}
