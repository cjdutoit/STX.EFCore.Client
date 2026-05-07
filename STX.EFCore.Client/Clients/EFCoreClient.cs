// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using STX.EFCore.Client.Brokers.Storages;
using STX.EFCore.Client.Services.Foundations.Operations;

namespace STX.EFCore.Client.Clients
{
    /// <summary>
    /// An EF Core client that wraps common data operations for use in a Storage Broker.
    /// Pass your <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to the constructor and
    /// delegate all CRUD and bulk operations to this client.
    /// </summary>
    public class EFCoreClient : IEFCoreClient
    {
        private readonly IOperationService operationService;

        /// <summary>
        /// Initialises a new instance of <see cref="EFCoreClient"/> using the supplied
        /// <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
        /// </summary>
        /// <param name="dbContext">
        /// The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to use for all operations.
        /// Must not be <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dbContext"/> is <see langword="null"/>.
        /// </exception>
        public EFCoreClient(DbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            IServiceProvider serviceProvider = RegisterServices(dbContext);
            this.operationService = serviceProvider.GetRequiredService<IOperationService>();
        }

        internal EFCoreClient(IOperationService operationService) =>
            this.operationService = operationService;

        /// <inheritdoc/>
        public async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.InsertAsync(@object, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.SelectAllAsync<T>(cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.SelectAsync<T>(objectIds, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.UpdateAsync(@object, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.DeleteAsync(@object, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask BulkInsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.BulkInsertAsync(objects, useTransaction, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(
            IEnumerable<T> objects,
            CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.BulkReadAsync(objects, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask BulkUpdateAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.BulkUpdateAsync(objects, useTransaction, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask BulkDeleteAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.BulkDeleteAsync(objects, useTransaction, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask BulkUpsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.BulkUpsertAsync(objects, useTransaction, cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync<T>(
            object[] objectIds,
            CancellationToken cancellationToken = default)
            where T : class =>
                await this.operationService.ExistsAsync<T>(objectIds, cancellationToken);

        private static IServiceProvider RegisterServices(DbContext dbContext)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient(_ => dbContext)
                .AddTransient<IStorageBroker, StorageBroker>()
                .AddTransient<IOperationService, OperationService>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}

