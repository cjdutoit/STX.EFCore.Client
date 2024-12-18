﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace STX.EFCore.Client.Brokers.Storages
{
    internal interface IStorageBroker
    {
        ValueTask SaveChangesAsync();
        ValueTask<IDbContextTransaction> BeginTransactionAsync();
        ValueTask<IEntityType> FindEntityType<T>();
        ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class;
        ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class;
        ValueTask UpdateObjectStateAsync<T>(T @object, EntityState entityState) where T : class;
        ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class;
    }
}
