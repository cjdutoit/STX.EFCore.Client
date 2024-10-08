// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Brokers.Storages;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    internal class OperationService : IOperationService
    {
        private readonly IStorageBroker storageBroker;

        public OperationService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public async ValueTask<T> InsertAsync<T>(T @object) where T : class
        {
            try
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Added);
                await storageBroker.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
            }
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            await storageBroker.SelectAllAsync<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await storageBroker.SelectAsync<T>(objectIds);

        public async ValueTask<T> UpdateAsync<T>(T @object) where T : class
        {
            try
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Modified);
                await storageBroker.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
            }
        }

        public async ValueTask<T> DeleteAsync<T>(T @object) where T : class
        {
            try
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Deleted);
                await storageBroker.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
            }
        }

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects, bool useTransaction = true) where T : class
        {
            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync();

                try
                {
                    await storageBroker.BulkInsertAsync(objects);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                await storageBroker.BulkInsertAsync(objects);
            }
        }

        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(IEnumerable<T> objects) where T : class =>
            await storageBroker.BulkReadAsync(objects);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects, bool useTransaction = true) where T : class
        {
            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync();

                try
                {
                    await storageBroker.BulkUpdateAsync(objects);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                await storageBroker.BulkUpdateAsync(objects);
            }
        }

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects, bool useTransaction = true) where T : class
        {
            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync();

                try
                {
                    await storageBroker.BulkDeleteAsync(objects);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                await storageBroker.BulkDeleteAsync(objects);
            }
        }
    }
}
