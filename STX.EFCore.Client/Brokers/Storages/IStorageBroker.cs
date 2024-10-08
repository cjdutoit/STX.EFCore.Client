// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace STX.EFCore.Client.Brokers.Storages
{
    internal interface IStorageBroker
    {
        ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class;
        ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class;
        ValueTask UpdateObjectStateAsync<T>(T @object, EntityState entityState) where T : class;
        ValueTask SaveChangesAsync();
        ValueTask<IDbContextTransaction> BeginTransactionAsync();
        ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask<IEnumerable<T>> BulkReadAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class;
    }
}
