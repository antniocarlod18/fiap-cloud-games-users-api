using FiapCloudGames.Contracts.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using MediatR;

namespace FiapCloudGamesUsers.Domain.Events
{
    public class UserUnlockedDomainEventHandler : INotificationHandler<UserUnlockedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserUnlockedDomainEventHandler> _logger;

        public UserUnlockedDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<UserUnlockedDomainEventHandler> logger)
        {
            this._publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(UserUnlockedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("UserUnlockedDomainEventHandler: Handling UserUnlockedDomainEvent for {UserId}", notification.UserId);
            await _publishEndpoint.Publish<UserUnlockedIntegrationEvent>(new()
            {
                UserId = notification.UserId,
                Name = notification.Name,
                Email = notification.Email
            });

            _logger?.LogInformation("UserUnlockedDomainEventHandler: Published UserUnlockedIntegrationEvent for {UserId}", notification.UserId);

            return;
        }
    }
}
