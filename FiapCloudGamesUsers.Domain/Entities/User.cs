using FiapCloudGamesUsers.Domain.Aggregates;
using FiapCloudGamesUsers.Domain.Events;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace FiapCloudGamesUsers.Domain.Entities;

public class User : AggregateRoot
{
    public required string Name { get; set; }
    public required string HashPassword { get; set; }
    public required string Email { get; set; }
    public required bool Active { get; set; }
    public required bool IsAdmin { get; set; }

    [SetsRequiredMembers]
    public User(string name, string hashPassword, string email, string password) : base()
    {
        Name = name;
        HashPassword = hashPassword;
        Email = email;
        Active = false;
        AddDomainEvent(new UserCreatedDomainEvent(Id, Name, Email, password));
    }

    [SetsRequiredMembers]
    protected User()
    {
    }

    public void UnlockAccount(string hashPassword)
    {
        HashPassword = hashPassword;
        Active = true;
        DateUpdated = DateTime.UtcNow;
        AddDomainEvent(new UserUnlockedDomainEvent(Id, Name, Email));
    }

    public void LockUser()
    {
        Active = false;
        HashPassword = "";
        DateUpdated = DateTime.UtcNow;
        AddDomainEvent(new UserLockedDomainEvent(Id, Name, Email));
    }

    public void MakeAdmin()
    {
        IsAdmin = true;
        DateUpdated = DateTime.UtcNow;
    }

    public void RevokeAdmin()
    {
        IsAdmin = false;
        DateUpdated = DateTime.UtcNow;
    }    
}
