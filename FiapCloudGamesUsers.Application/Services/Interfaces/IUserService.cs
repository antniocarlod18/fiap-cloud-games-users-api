using FiapCloudGamesUsers.Application.Dtos;

namespace FiapCloudGamesUsers.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> AddAsync(UserRequestDto dto);
    Task<UserResponseDto?> GetAsync(Guid id);
    Task<UserResponseDto?> UnlockAsync(Guid id, UserUnlockRequestDto userUnlockRequestDto);
    Task<UserResponseDto?> MakeAdminAsync(Guid id);
    Task<UserResponseDto?> RevokeAdminAsync(Guid id);
    Task<IList<UserResponseDto?>> GetAllAsync();
    Task DeleteAsync(Guid id);
    Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateRequestDto dto);
    Task<UserResponseDto?> AuthenticateAsync(UserAuthenticateRequestDto dto);
}