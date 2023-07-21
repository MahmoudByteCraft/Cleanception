using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Common.Messaging;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Result<Guid>>
    where TCommand : ICommand
{
}

