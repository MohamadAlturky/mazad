using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mazad.Core.Shared.Interfaces;

public interface IUnitOfWork : IDisposable
{
    DbContext Context { get; }
    IDbTransaction BeginTransaction();
    Task<IDbTransaction> BeginTransactionAsync();
    void Commit();
    Task CommitAsync();
    void Rollback();
    Task RollbackAsync();
    Task SaveChangesAsync();
} 