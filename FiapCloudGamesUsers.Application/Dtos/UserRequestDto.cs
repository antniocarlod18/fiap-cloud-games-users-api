namespace FiapCloudGamesUsers.Application.Dtos;

public class UserRequestDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class UserUpdateRequestDto
{
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
}