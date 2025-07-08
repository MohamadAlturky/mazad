using Mazad.Core.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Mazad.Core.Shared;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public DbContext Context => _context;

    public IDbTransaction BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction().GetDbTransaction();
        return _transaction;
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        _transaction = transaction.GetDbTransaction();
        return _transaction;
    }

    public void Commit()
    {
        try
        {
            _context.SaveChanges();
            _transaction?.Commit();
        }
        catch
        {
            Rollback();
            throw;
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _context.Database.CurrentTransaction!.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    public void Rollback()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.RollbackAsync();
            }
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }
} 