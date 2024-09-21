// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace STX.EFCore.Client.Services.Foundations
{
    public class OperationService : IOperationService
    {
        private readonly DbContext dbContext;

        public OperationService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async ValueTask<T> InsertAsync<T>(T @object) where T : class
        {
            dbContext.Entry(@object).State = EntityState.Added;
            await dbContext.SaveChangesAsync();
            dbContext.Entry(@object).State = EntityState.Detached;

            return @object;
        }

        public ValueTask<IQueryable<T>> ReadAllAsync<T>() where T : class =>
            throw new NotImplementedException();

        public ValueTask<T> ReadAsync<T>(params object[] objectIds) where T : class =>
            throw new NotImplementedException();

        public ValueTask<T> UpdateAsync<T>(T @object) where T : class =>
            throw new NotImplementedException();

        public ValueTask<T> DeleteAsync<T>(T @object) where T : class =>
            throw new NotImplementedException();

        public ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            throw new NotImplementedException();

        public ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            throw new NotImplementedException();
    }
}
