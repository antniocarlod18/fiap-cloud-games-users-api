using FiapCloudGames.Contracts.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using MediatR;

namespace FiapCloudGamesUsers.Domain.Events
{
    public class UserLockedEventHandler : INotificationHandler<UserLockedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserLockedEventHandler> _logger;

        public UserLockedEventHandler(IPublishEndpoint publishEndpoint, ILogger<UserLockedEventHandler> logger)
        {
            this._publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(UserLockedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("UserLockedEventHandler: Handling UserLockedDomainEvent for {UserId}", notification.UserId);
            await _publishEndpoint.Publish<UserLockedIntegrationEvent>(new()
            {
                UserId = notification.UserId,
                Name = notification.Name,
                Email = notification.Email
            });

            _logger?.LogInformation("UserLockedEventHandler: Published UserLockedIntegrationEvent for {UserId}", notification.UserId);

            return;
        }
    }
}
