using FiapCloudGamesUsers.Domain.Entities;

namespace FiapCloudGamesUsers.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IList<User>> GetActiveAsync();
}   