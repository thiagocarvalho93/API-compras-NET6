namespace ApiDotnet.Domain.Repositories
{
    public interface IUnityOfWork : IDisposable
    {
        Task BeginTransaction();
        Task Commit();
        Task Rollback();
    }
}