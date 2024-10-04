// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    internal interface IOperationService
    {
        ValueTask<T> InsertAsync<T>(T @object) where T : class;
        ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class;
        ValueTask<T> SelectAsync<T>(params object[] @objectIds) where T : class;
        ValueTask<T> UpdateAsync<T>(T @object) where T : class;
        ValueTask<T> DeleteAsync<T>(T @object) where T : class;
        ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask<IEnumerable<T>> BulkReadAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class;
    }
}
