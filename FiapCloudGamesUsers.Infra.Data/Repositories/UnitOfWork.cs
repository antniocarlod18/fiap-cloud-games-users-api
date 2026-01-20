using FiapCloudGamesUsers.Domain.Repositories;
using FiapCloudGamesUsers.Infra.Data.Context;

namespace FiapCloudGamesUsers.Infra.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ContextDb _context;
    private IUserRepository _userRepository;

    public UnitOfWork(ContextDb contextDb)
    {
        _context = contextDb;
    }

    public IUserRepository UsersRepo
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository = new UserRepository(_context);
            }
            return _userRepository;
        }
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
