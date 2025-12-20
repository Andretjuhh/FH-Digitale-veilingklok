using Application.Common.Exceptions;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            throw CustomException.ExistingTransaction();
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Save changes within the transaction
            await SaveChangesAsync(cancellationToken);

            // Commit the actual database transaction
            if (_currentTransaction != null)
                await _currentTransaction.CommitAsync();
        }
        catch
        {
            // Ensure rollback happens if anything fails
            await RollbackAsync();
            throw;
        }
        finally
        {
            // Clean up the transaction object
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Rollback the transaction
            if (_currentTransaction != null)
                await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            // Clean up the transaction object
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        // Dispose the current transaction if it exists
        _currentTransaction?.Dispose();
        // _dbContext.Dispose(); // Managed by DI container
        GC.SuppressFinalize(this);
    }
}
