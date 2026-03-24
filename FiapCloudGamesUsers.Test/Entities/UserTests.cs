using System;
using Xunit;
using FiapCloudGamesUsers.Domain.Entities;

namespace FiapCloudGamesUsers.Test.Entities;

public class UserTests
{
    [Fact]
    public void Creating_user_should_add_created_domain_event_and_set_properties()
    {
        var user = new User("John Doe", "hash", "john@example.com", "password");
        Assert.Equal("John Doe", user.Name);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("hash", user.HashPassword);
        Assert.False(user.Active);
        Assert.Single(user.DomainEvents);
        Assert.Single(user.DomainEvents, e => e is FiapCloudGamesUsers.Domain.Events.UserCreatedDomainEvent);
    }

    [Fact]
    public void Unlocking_user_should_set_active_and_add_event()
    {
        var user = new User("John Doe", "hash", "john@example.com", "password");

        user.UnlockAccount("newhash");

        Assert.True(user.Active);
        Assert.Equal("newhash", user.HashPassword);
        Assert.Contains(user.DomainEvents, e => e is FiapCloudGamesUsers.Domain.Events.UserUnlockedDomainEvent);
    }

    [Fact]
    public void Locking_user_should_set_inactive_and_add_event()
    {
        var user = new User("John Doe", "hash", "john@example.com", "password");

        user.LockUser();

        Assert.False(user.Active);
        Assert.Equal(string.Empty, user.HashPassword);
        Assert.Contains(user.DomainEvents, e => e is FiapCloudGamesUsers.Domain.Events.UserLockedDomainEvent);
    }

    [Fact]
    public void MakeAdmin_and_RevokeAdmin_should_toggle_admin_flag()
    {
        var user = new User("John Doe", "hash", "john@example.com", "password");

        user.MakeAdmin();
        Assert.True(user.IsAdmin);

        user.RevokeAdmin();
        Assert.False(user.IsAdmin);
    }
}
