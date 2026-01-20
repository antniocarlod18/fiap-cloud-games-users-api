using FiapCloudGamesUsers.Domain.Entities;
using FiapCloudGamesUsers.Domain.Events;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesUsers.Domain.Aggregates;

public abstract class AggregateRoot : EntityBase
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [SetsRequiredMembers]
    protected AggregateRoot() : base()
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
