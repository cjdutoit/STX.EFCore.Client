// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Brokers.Storages;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    internal partial class OperationService : IOperationService
    {
        private readonly IStorageBroker storageBroker;

        public OperationService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default) where T
            : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateObjectIsNotNull(@object);

                try
                {
                    await storageBroker.UpdateObjectStateAsync(@object, EntityState.Added);
                    await storageBroker.SaveChangesAsync(cancellationToken);

                    return @object;
                }
                finally
                {
                    await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            });

        public ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default)
            where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await storageBroker.SelectAllAsync<T>();
            });

        public ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken = default)
            where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateObjectIdsIsNotNull(objectIds);

                return await storageBroker.SelectAsync<T>(objectIds, cancellationToken);
            });

        public ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateObjectIsNotNull(@object);

                try
                {
                    await storageBroker.UpdateObjectStateAsync(@object, EntityState.Modified);
                    await storageBroker.SaveChangesAsync(cancellationToken);

                    return @object;
                }
                finally
                {
                    await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            });

        public ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateObjectIsNotNull(@object);

                try
                {
                    await storageBroker.UpdateObjectStateAsync(@object, EntityState.Deleted);
                    await storageBroker.SaveChangesAsync(cancellationToken);

                    return @object;
                }
                finally
                {
                    await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            });

        public ValueTask BulkInsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default) where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateCollectionIsNotNull(objects);

                if (useTransaction)
                {
                    using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        await storageBroker.BulkInsertAsync(objects, cancellationToken);
                        await storageBroker.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                    finally
                    {
                        foreach (var @object in objects)
                        {
                            await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await storageBroker.BulkInsertAsync(objects, cancellationToken);
                        await storageBroker.SaveChangesAsync(cancellationToken);
                    }
                    finally
                    {
                        foreach (var @object in objects)
                        {
                            await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                        }
                    }
                }
            });

        public ValueTask<IEnumerable<T>> BulkReadAsync<T>(
            IEnumerable<T> objects,
            CancellationToken cancellationToken = default) where T : class =>
            TryCatch((ReturningCollectionFunction<T>)(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateCollectionIsNotNull(objects);

                var entityType = await this.storageBroker.FindEntityTypeAsync<T>();
                var keyProperty = entityType?.FindPrimaryKey()?.Properties?.FirstOrDefault();

                if (keyProperty == null)
                {
                    throw new InvalidOperationException($"No primary key defined for entity {typeof(T).Name}");
                }

                var keyValues = objects
                    .Select(obj => keyProperty.PropertyInfo.GetValue(obj))
                    .Where(key => key != null)
                    .ToList();

                var listOfKeyValues = keyValues.Cast<object>().ToList();
                var parameter = Expression.Parameter(type: typeof(T), name: "e");
                var property = Expression.Property(expression: parameter, propertyName: keyProperty.Name);
                var containsMethod = typeof(List<object>).GetMethod("Contains");

                var body = Expression.Call(
                    instance: Expression.Constant(listOfKeyValues),
                    method: containsMethod,
                    arguments: Expression.Convert(property, typeof(object)));

                var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
                var query = await storageBroker.SelectAllAsync<T>();

                return (IEnumerable<T>)query.Where(predicate).ToList();
            }));


        public ValueTask BulkUpdateAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default) where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateCollectionIsNotNull(objects);

                if (useTransaction)
                {
                    using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        await storageBroker.BulkUpdateAsync(objects, cancellationToken);
                        await storageBroker.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                    finally
                    {
                        foreach (var @object in objects)
                        {
                            await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await storageBroker.BulkUpdateAsync(objects, cancellationToken);
                        await storageBroker.SaveChangesAsync(cancellationToken);
                    }
                    finally
                    {
                        foreach (var @object in objects)
                        {
                            await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                        }
                    }
                }
            });

        public ValueTask BulkDeleteAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default) where T : class =>
            TryCatch(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ValidateCollectionIsNotNull(objects);

                if (useTransaction)
                {
                    using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        await storageBroker.BulkDeleteAsync(objects, cancellationToken);
                        await storageBroker.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                    finally
                    {
                        foreach (var @object in objects)
                        {
                            await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await storageBroker.BulkDeleteAsync(objects, cancellationToken);
                        await storageBroker.SaveChangesAsync(cancellationToken);
                    }
                    finally
                    {
                        foreach (var @object in objects)
                        {
                            await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                        }
                    }
                }
            });
    }
}
