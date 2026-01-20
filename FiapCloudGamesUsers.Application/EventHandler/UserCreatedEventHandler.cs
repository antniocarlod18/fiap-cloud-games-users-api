using FiapCloudGames.Contracts.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using MediatR;

namespace FiapCloudGamesUsers.Domain.Events
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(IPublishEndpoint publishEndpoint, ILogger<UserCreatedEventHandler> logger)
        {
            this._publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("UserCreatedEventHandler: Handling UserCreatedDomainEvent for user {UserId}", notification.UserId);
            await _publishEndpoint.Publish<UserCreatedIntegrationEvent>(new()
            {
                UserId = notification.UserId,
                Name = notification.Name,
                Email = notification.Email,
                Password = notification.Password,
            });

            _logger?.LogInformation("UserCreatedEventHandler: Published UserCreatedIntegrationEvent for user {UserId}", notification.UserId);
            return;
        }
    }
}
