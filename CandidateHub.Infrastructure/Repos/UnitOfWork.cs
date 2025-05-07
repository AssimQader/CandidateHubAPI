using CandidateHub.Domain.Interfaces.Repos;
using CandidateHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Infrastructure.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private readonly CandidateHubDbContext _context;

        public UnitOfWork(CandidateHubDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.TryGetValue(typeof(T), out var repo))
            {
                return (IGenericRepository<T>)repo;
            }

            // Resolve the generic repo from the DI container
            IGenericRepository<T> repository = (IGenericRepository<T>)Activator.CreateInstance(
                typeof(GenericRepository<>).MakeGenericType(typeof(T)), _context)!;

            _repositories.TryAdd(typeof(T), repository); 

            return repository;
        }




        public async Task<int> CommitAsync()
        {
            // check if a transaction is already active (EF Core allows nesting)
            if (_context.Database.CurrentTransaction != null)
            {
                //just call SaveChangesAsync() without opening a new transaction
                return await _context.SaveChangesAsync();
            }


            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // We can include pre-commit logic hooks here if needed
                // Example: audit tracking, domain events dispatch, etc.

                int result = await _context.SaveChangesAsync();

                // Post-commit logic hooks can go here as well
                // Example: Logging or publishing integration events

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Optional logging
                // _logger.LogError(ex, "Transaction rolled back due to error");

                throw;
            }
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public void Rollback()
        {
            try
            {
                // Quick exit if there are no changes
                if (!_context.ChangeTracker.HasChanges())
                    return;

                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    if (entry == null || entry.Entity == null)
                        continue;

                    switch (entry.State)
                    {
                        case EntityState.Modified:
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            break;

                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;

                        case EntityState.Detached:
                        case EntityState.Unchanged:
                            // No rollback needed
                            break;

                        default:
                            // Log unknown states
                            // _logger.LogWarning($"Unhandled entity state during rollback: {entry.State}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // log internal rollback failure (but do not rethrow)
                // _logger.LogError(ex, "An error occurred while rolling back tracked entity changes.");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
