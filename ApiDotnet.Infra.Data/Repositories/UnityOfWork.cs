using ApiDotnet.Domain.Repositories;
using ApiDotnet.Infra.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApiDotnet.Infra.Data.Repositories
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly ApplicationDbContext _db;
        private IDbContextTransaction _transaction;

        public UnityOfWork(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task BeginTransaction()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}