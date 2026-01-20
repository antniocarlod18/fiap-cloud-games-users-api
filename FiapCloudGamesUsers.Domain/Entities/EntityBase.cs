using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesUsers.Domain.Entities;

public class EntityBase
{
    public required Guid Id { get; set; }
    public required DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }

    [SetsRequiredMembers]
    public EntityBase()
    {
        Id = Guid.NewGuid();
        DateCreated = DateTime.UtcNow;
        DateUpdated = null;
    }
}
