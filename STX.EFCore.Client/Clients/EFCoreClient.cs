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
    public class EFCoreClient : IEFCoreClient
    {
        private readonly IOperationService operationService;

        public EFCoreClient(DbContext dbContext)
        {
            IServiceProvider serviceProvider = RegisterServices(dbContext);
            this.operationService = serviceProvider.GetRequiredService<IOperationService>();
        }

        public async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.InsertAsync(@object, cancellationToken);

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.SelectAllAsync<T>(cancellationToken);

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await this.operationService.SelectAsync<T>(objectIds);

        public async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken) where T : class =>
            await this.operationService.SelectAsync<T>(objectIds, cancellationToken);

        public async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.UpdateAsync(@object, cancellationToken);

        public ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default) where T : class =>
            this.operationService.DeleteAsync(@object, cancellationToken);

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects, bool useTransaction = true, CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.BulkInsertAsync(objects, useTransaction, cancellationToken);

        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.BulkReadAsync(objects, cancellationToken);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects, bool useTransaction = true, CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.BulkUpdateAsync(objects, useTransaction, cancellationToken);

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects, bool useTransaction = true, CancellationToken cancellationToken = default) where T : class =>
            await this.operationService.BulkDeleteAsync(objects, useTransaction, cancellationToken);

        private static IServiceProvider RegisterServices(DbContext dbContext)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient(_ => dbContext)
                .AddTransient<IStorageBroker, StorageBroker>()
                .AddTransient<IOperationService, OperationService>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
