using FiapCloudGamesUsers.Domain.Entities;

namespace FiapCloudGamesUsers.Application.Dtos;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Active { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }

    public static implicit operator UserResponseDto?(User? user)
    {
        if (user == null) return null;
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Active = user.Active,
            IsAdmin = user.IsAdmin,
            DateCreated = user.DateCreated,
            DateUpdated = user.DateUpdated
        };
    }
}