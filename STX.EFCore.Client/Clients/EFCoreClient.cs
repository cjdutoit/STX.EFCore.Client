// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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

        public async ValueTask<T> InsertAsync<T>(T @object) where T : class =>
            await this.operationService.InsertAsync(@object);

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            await this.operationService.SelectAllAsync<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await this.operationService.SelectAsync<T>(objectIds);

        public async ValueTask<T> UpdateAsync<T>(T @object) where T : class =>
            await this.operationService.UpdateAsync(@object);

        public ValueTask<T> DeleteAsync<T>(T @object) where T : class =>
            this.operationService.DeleteAsync(@object);

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.operationService.BulkInsertAsync(objects);

        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.operationService.BulkReadAsync(objects);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.operationService.BulkUpdateAsync(objects);

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.operationService.BulkDeleteAsync(objects);

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
