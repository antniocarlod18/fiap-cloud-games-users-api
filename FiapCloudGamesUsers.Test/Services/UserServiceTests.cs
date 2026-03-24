using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FiapCloudGamesUsers.Application.Services;
using FiapCloudGamesUsers.Domain.Repositories;
using FiapCloudGamesUsers.Application.Dtos;
using FiapCloudGamesUsers.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiapCloudGamesUsers.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ILogger<UserService>> _loggerMock = new();

    public UserServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.UsersRepo).Returns(_userRepoMock.Object);
    }

    [Fact]
    public async Task AddAsync_should_create_user_and_call_commit()
    {
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        var service = new UserService(_unitOfWorkMock.Object, _loggerMock.Object);

        var dto = new UserRequestDto { Name = "John", Email = "john@example.com" };

        var result = await service.AddAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_should_throw_when_not_found()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var service = new UserService(_unitOfWorkMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<FiapCloudGamesUsers.Domain.Exceptions.ResourceNotFoundException>(() => service.GetAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AuthenticateAsync_should_throw_when_invalid()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var service = new UserService(_unitOfWorkMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<FiapCloudGamesUsers.Domain.Exceptions.AuthorizationException>(() => service.AuthenticateAsync(new UserAuthenticateRequestDto { Email = "a@b.com", Password = "p" }));
    }

    [Fact]
    public async Task GetAllAsync_should_return_list()
    {
        var users = new List<User> { new User("John", "h", "j@e.com", "p") };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var service = new UserService(_unitOfWorkMock.Object, _loggerMock.Object);

        var result = await service.GetAllAsync();

        Assert.Single(result);
    }
}
