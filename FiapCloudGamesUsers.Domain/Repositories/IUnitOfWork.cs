namespace FiapCloudGamesUsers.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UsersRepo { get; }
        Task Commit();
    }
}
