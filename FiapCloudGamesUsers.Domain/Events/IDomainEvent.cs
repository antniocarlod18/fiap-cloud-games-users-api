namespace FiapCloudGamesUsers.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
