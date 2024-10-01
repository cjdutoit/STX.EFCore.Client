// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Services.Foundations;

namespace STX.EFCore.Client.Clients
{
    public class EFCoreClient : IEFCoreClient
    {
        private readonly IOperationService operationService;

        public EFCoreClient(DbContext dbContext)
        {
            this.operationService = new OperationService(dbContext);
        }

        public async ValueTask<T> InsertAsync<T>(T @object) where T : class =>
            await this.operationService.InsertAsync(@object);

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.operationService.BulkInsertAsync(objects);

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            await this.operationService.SelectAllAsync<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await this.operationService.SelectAsync<T>(objectIds);

        public async ValueTask<T> UpdateAsync<T>(T @object) where T : class =>
            await this.operationService.UpdateAsync(@object);

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await this.operationService.BulkUpdateAsync(objects);

        public ValueTask<T> DeleteAsync<T>(T @object) where T : class =>
            this.operationService.DeleteAsync(@object);
    }
}
