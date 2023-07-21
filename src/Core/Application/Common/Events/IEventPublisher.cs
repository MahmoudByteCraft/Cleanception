using Cleanception.Application.Common.Interfaces;
using Cleanception.Shared.Events;

namespace Cleanception.Application.Common.Events;

public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}