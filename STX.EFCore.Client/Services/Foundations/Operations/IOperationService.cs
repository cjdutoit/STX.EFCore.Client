// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STX.EFCore.Client.Services.Foundations
{
    internal interface IOperationService
    {
        ValueTask<T> InsertAsync<T>(T @object) where T : class;
        ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask<IQueryable<T>> ReadAllAsync<T>() where T : class;
        ValueTask<T> ReadAsync<T>(params object[] @objectIds) where T : class;
        ValueTask<T> UpdateAsync<T>(T @object) where T : class;
        ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class;
        ValueTask<T> DeleteAsync<T>(T @object) where T : class;
    }
}
