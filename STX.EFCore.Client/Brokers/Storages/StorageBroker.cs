// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace STX.EFCore.Client.Brokers.Storages
{
    internal class StorageBroker : IStorageBroker
    {
        private readonly DbContext dbContext;

        public StorageBroker(DbContext dbContext) =>
            this.dbContext = dbContext;

        public async ValueTask<IEntityType> FindEntityTypeAsync<T>() =>
            this.dbContext.Model.FindEntityType(typeof(T));

        public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await this.dbContext.SaveChangesAsync(cancellationToken);

        public async ValueTask<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default) =>
                await this.dbContext.Database.BeginTransactionAsync(cancellationToken);

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            this.dbContext.Set<T>();

        public async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken = default)
            where T : class =>
                await this.dbContext.FindAsync<T>(objectIds, cancellationToken);

        public async ValueTask UpdateObjectStateAsync<T>(T @object, EntityState entityState)
            where T : class =>
                this.dbContext.Entry(@object).State = entityState;

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default)
            where T : class =>
                await this.dbContext.AddRangeAsync(objects, cancellationToken);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default)
            where T : class =>
                this.dbContext.UpdateRange(objects);

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default)
            where T : class =>
            this.dbContext.RemoveRange(objects);
    }
}
