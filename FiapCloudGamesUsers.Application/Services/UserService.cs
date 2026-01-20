using FiapCloudGamesUsers.Application.Dtos;
using FiapCloudGamesUsers.Application.Services.Interfaces;
using FiapCloudGamesUsers.Domain.Entities;
using FiapCloudGamesUsers.Domain.Exceptions;
using FiapCloudGamesUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiapCloudGamesUsers.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
    }

    public async Task<UserResponseDto?> AddAsync(UserRequestDto dto)
    {
        _logger.LogInformation("Starting AddAsync for user '{Name}'", dto.Name);

        var passwordRandom = GenerateRandomPassword();
        var hashedPassword = HashPassword(passwordRandom);

        var user = new User(dto.Name, hashedPassword, dto.Email, passwordRandom);

        await _unitOfWork.UsersRepo.AddAsync(user);
        await _unitOfWork.Commit();
        _logger.LogInformation("User created with id {UserId}", user.Id);
        return user;
    }

    public async Task<UserResponseDto?> GetAsync(Guid id)
    {
        _logger.LogInformation("Getting user by id {UserId}", id);
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if(user == null)
        {
            _logger.LogWarning("User with id {UserId} not found", id);
            throw new ResourceNotFoundException(nameof(User));
        }

        _logger.LogInformation("User with id {UserId} retrieved", id);
        return user;
    }

    public async Task<UserResponseDto?> UnlockAsync(Guid id, UserUnlockRequestDto userUnlockRequestDto)
    {
        _logger.LogInformation("Unlocking user {UserId}", id);
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when trying to unlock", id);
            throw new ResourceNotFoundException(nameof(User));
        }

        var hashedPassword = HashPassword(userUnlockRequestDto.Password);
        user.UnlockAccount(hashedPassword);
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        _logger.LogInformation("User {UserId} unlocked", id);
        return user;
    }

    public async Task<UserResponseDto?> MakeAdminAsync(Guid id)
    {
        _logger.LogInformation("Granting admin to user {UserId}", id);
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when granting admin", id);
            throw new ResourceNotFoundException(nameof(User));
        }

        user.MakeAdmin();
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        _logger.LogInformation("User {UserId} granted admin", id);
        return user;
    }

    public async Task<UserResponseDto?> RevokeAdminAsync(Guid id)
    {
        _logger.LogInformation("Revoking admin from user {UserId}", id);
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when revoking admin", id);
            throw new ResourceNotFoundException(nameof(User));
        }

        user.RevokeAdmin();
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        _logger.LogInformation("User {UserId} admin revoked", id);
        return user;
    }

    public async Task<IList<UserResponseDto?>> GetAllAsync()
    {
        _logger.LogInformation("Getting all users");
        var usersEnumerable = await _unitOfWork.UsersRepo.GetAllAsync();
        var users = usersEnumerable.ToList();
        _logger.LogInformation("Retrieved {Count} users", users.Count);
        return users.Select(x => (UserResponseDto?)x).ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Locking (soft-delete) user {UserId}", id);
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when trying to lock", id);
            throw new ResourceNotFoundException(nameof(User));
        }

        user.LockUser();

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        _logger.LogInformation("User {UserId} locked", id);
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateRequestDto dto)
    {
        _logger.LogInformation("Updating user {UserId}", id);
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);
        
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when trying to update", id);
            throw new ResourceNotFoundException(nameof(User));
        }

        user.Name = dto.Name;
        user.HashPassword = HashPassword(dto.Password);
        user.DateUpdated = DateTime.UtcNow;

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        _logger.LogInformation("User {UserId} updated", id);
        return user;
    }

    public async Task<UserResponseDto?> AuthenticateAsync(UserAuthenticateRequestDto dto)
    {
        _logger.LogInformation("Authenticating user with email {Email}", dto.Email);
        var user = await _unitOfWork.UsersRepo.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            _logger.LogWarning("Authentication failed for email {Email}: user not found", dto.Email);
            throw new AuthorizationException();
        }

        if(!VerifyPassword(user.HashPassword, dto.Password))
        {
            _logger.LogWarning("Authentication failed for email {Email}: invalid password", dto.Email);
            throw new AuthorizationException();
        }

        _logger.LogInformation("User {UserId} authenticated successfully", user.Id);
        return user;
    }

    private string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
    }

    private string HashPassword(string password)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(plainTextBytes).ToString();
    }

    private bool VerifyPassword(string hashedPassword, string password)
    {
        var hashedInputPassword = HashPassword(password);
        return hashedPassword == hashedInputPassword;
    }
}