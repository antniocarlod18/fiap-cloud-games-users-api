using FiapCloudGamesUsers.Domain.Entities;
using FiapCloudGamesUsers.Domain.Repositories;
using FiapCloudGamesUsers.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesUsers.Infra.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ContextDb _context;

    public UserRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IList<User>> GetActiveAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Active)
            .ToListAsync();
    }
}
