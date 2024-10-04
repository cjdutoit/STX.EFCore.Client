// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Z.EntityFramework.Extensions;

namespace STX.EFCore.Client.Brokers.Storages
{
    internal class StorageBroker : IStorageBroker
    {
        private readonly DbContext dbContext;

        public StorageBroker(DbContext dbContext)
        {
            this.dbContext = dbContext;
            EntityFrameworkManager.ContextFactory = context => this.dbContext;
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            this.dbContext.Set<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await this.dbContext.FindAsync<T>(objectIds);

        public async ValueTask UpdateObjectStateAsync<T>(T @object, EntityState entityState) where T : class =>
            this.dbContext.Entry(@object).State = entityState;

        public async ValueTask SaveChangesAsync() =>
            await this.dbContext.SaveChangesAsync();

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.dbContext.BulkInsertAsync(objects);

        public async ValueTask<List<T>> BulkReadAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.dbContext.Set<T>().BulkReadAsync(objects);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.dbContext.BulkUpdateAsync(objects);

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.dbContext.BulkDeleteAsync(objects);
    }
}
