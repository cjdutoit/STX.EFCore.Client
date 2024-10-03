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
                storageBroker.DbContext.Entry(@object).State = EntityState.Added;
                await storageBroker.DbContext.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                storageBroker.DbContext.Entry(@object).State = EntityState.Detached;
            }
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            storageBroker.DbContext.Set<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await storageBroker.DbContext.FindAsync<T>(objectIds);

        public async ValueTask<T> UpdateAsync<T>(T @object) where T : class
        {
            try
            {
                storageBroker.DbContext.Entry(@object).State = EntityState.Modified;
                await storageBroker.DbContext.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                storageBroker.DbContext.Entry(@object).State = EntityState.Detached;
            }
        }

        public async ValueTask<T> DeleteAsync<T>(T @object) where T : class
        {
            try
            {
                storageBroker.DbContext.Entry(@object).State = EntityState.Deleted;
                await storageBroker.DbContext.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                storageBroker.DbContext.Entry(@object).State = EntityState.Detached;
            }
        }

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await storageBroker.DbContext.BulkInsertAsync(objects);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await storageBroker.DbContext.BulkUpdateAsync(objects);

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class =>
            await storageBroker.DbContext.BulkDeleteAsync(objects);
    }
}
