using FiapCloudGamesUsers.Domain.Events;

namespace FiapCloudGamesUsers.Domain.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}
