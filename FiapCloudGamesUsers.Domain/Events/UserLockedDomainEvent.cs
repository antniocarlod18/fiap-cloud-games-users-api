using FiapCloudGamesUsers.Domain.Entities;
using MediatR;

namespace FiapCloudGamesUsers.Domain.Events
{
    public class UserLockedDomainEvent : IDomainEvent, INotification
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime OccurredOn { get; private set; }

        public UserLockedDomainEvent(Guid userId, string name, string email)
        {
            UserId = userId;
            Name = name;
            Email = email;
            OccurredOn = DateTime.UtcNow;
        }    
    }
}
