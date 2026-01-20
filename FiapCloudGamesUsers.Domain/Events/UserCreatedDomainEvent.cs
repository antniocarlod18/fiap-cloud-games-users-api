using FiapCloudGamesUsers.Domain.Entities;
using MediatR;

namespace FiapCloudGamesUsers.Domain.Events
{
    public class UserCreatedDomainEvent : IDomainEvent, INotification
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Password { get; set; }
        public string Email { get; private set; }
        public DateTime OccurredOn { get; private set; }

        public UserCreatedDomainEvent(Guid userId, string name, string email, string password)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Password = password;
            OccurredOn = DateTime.UtcNow;
        }    
    }
}
