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

        public async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(@object);
            cancellationToken.ThrowIfCancellationRequested();

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
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await storageBroker.SelectAllAsync<T>();
        }

        public async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objectIds);
            cancellationToken.ThrowIfCancellationRequested();

            return await storageBroker.SelectAsync<T>(objectIds, cancellationToken);
        }

        public async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(@object);
            cancellationToken.ThrowIfCancellationRequested();

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
        }

        public async ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(@object);
            cancellationToken.ThrowIfCancellationRequested();

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
        }

        public async ValueTask BulkInsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objects);
            cancellationToken.ThrowIfCancellationRequested();

            var objectList = objects.ToList();

            if (objectList.Count == 0)
                return;

            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                try
                {
                    await storageBroker.BulkInsertAsync(objectList, cancellationToken);
                    await storageBroker.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
            else
            {
                try
                {
                    await storageBroker.BulkInsertAsync(objectList, cancellationToken);
                    await storageBroker.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
        }

        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(
            IEnumerable<T> objects,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objects);
            cancellationToken.ThrowIfCancellationRequested();

            var objectList = objects.ToList();

            if (objectList.Count == 0)
                return Enumerable.Empty<T>();

            var entityType = await this.storageBroker.FindEntityTypeAsync<T>();
            var keyProperty = entityType?.FindPrimaryKey()?.Properties?.FirstOrDefault();

            if (keyProperty == null)
                throw new InvalidOperationException($"No primary key defined for entity {typeof(T).Name}");

            var keyValues = objectList
                .Select(obj => keyProperty.PropertyInfo.GetValue(obj))
                .Where(key => key != null)
                .Cast<object>()
                .ToList();

            if (keyValues.Count == 0)
                return Enumerable.Empty<T>();

            var parameter = Expression.Parameter(type: typeof(T), name: "e");
            var property = Expression.Property(expression: parameter, propertyName: keyProperty.Name);

            var typedKeyValues = keyValues
                .Select(k => Convert.ChangeType(k, keyProperty.ClrType))
                .ToList();

            var typedList = Expression.Constant(
                typeof(Enumerable)
                    .GetMethod(nameof(Enumerable.Cast))
                    .MakeGenericMethod(keyProperty.ClrType)
                    .Invoke(null, new object[] { typedKeyValues }));

            var containsMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                .MakeGenericMethod(keyProperty.ClrType);

            var body = Expression.Call(
                method: containsMethod,
                arg0: typedList,
                arg1: property);

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            var query = await storageBroker.SelectAllAsync<T>();

            return query.Where(predicate).ToList();
        }

        public async ValueTask BulkUpdateAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objects);
            cancellationToken.ThrowIfCancellationRequested();

            var objectList = objects.ToList();

            if (objectList.Count == 0)
                return;

            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                try
                {
                    await storageBroker.BulkUpdateAsync(objectList, cancellationToken);
                    await storageBroker.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
            else
            {
                try
                {
                    await storageBroker.BulkUpdateAsync(objectList, cancellationToken);
                    await storageBroker.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
        }

        public async ValueTask BulkDeleteAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objects);
            cancellationToken.ThrowIfCancellationRequested();

            var objectList = objects.ToList();

            if (objectList.Count == 0)
                return;

            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                try
                {
                    await storageBroker.BulkDeleteAsync(objectList, cancellationToken);
                    await storageBroker.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
            else
            {
                try
                {
                    await storageBroker.BulkDeleteAsync(objectList, cancellationToken);
                    await storageBroker.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
        }

        public async ValueTask BulkUpsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objects);
            cancellationToken.ThrowIfCancellationRequested();

            var objectList = objects.ToList();

            if (objectList.Count == 0)
                return;

            var entityType = await this.storageBroker.FindEntityTypeAsync<T>();
            var keyProperty = entityType?.FindPrimaryKey()?.Properties?.FirstOrDefault();

            if (keyProperty == null)
                throw new InvalidOperationException($"No primary key defined for entity {typeof(T).Name}");

            var parameter = Expression.Parameter(type: typeof(T), name: "e");
            var property = Expression.Property(expression: parameter, propertyName: keyProperty.Name);

            var typedKeyValues = objectList
                .Select(obj => keyProperty.PropertyInfo.GetValue(obj))
                .Where(key => key != null)
                .Select(k => Convert.ChangeType(k, keyProperty.ClrType))
                .ToList();

            var typedList = Expression.Constant(
                typeof(Enumerable)
                    .GetMethod(nameof(Enumerable.Cast))
                    .MakeGenericMethod(keyProperty.ClrType)
                    .Invoke(null, new object[] { typedKeyValues }));

            var containsMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                .MakeGenericMethod(keyProperty.ClrType);

            var body = Expression.Call(
                method: containsMethod,
                arg0: typedList,
                arg1: property);

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);

            if (useTransaction)
            {
                using var transaction = await storageBroker.BeginTransactionAsync(cancellationToken);

                try
                {
                    var query = await storageBroker.SelectAllAsync<T>();
                    var existingKeys = new HashSet<object>(
                        query.Where(predicate).ToList()
                            .Select(e => keyProperty.PropertyInfo.GetValue(e))
                            .Where(k => k != null)
                            .Cast<object>());

                    var toInsert = objectList
                        .Where(obj =>
                        {
                            var key = keyProperty.PropertyInfo.GetValue(obj);
                            return key == null || !existingKeys.Contains(key);
                        })
                        .ToList();

                    var toUpdate = objectList
                        .Where(obj =>
                        {
                            var key = keyProperty.PropertyInfo.GetValue(obj);
                            return key != null && existingKeys.Contains(key);
                        })
                        .ToList();

                    if (toInsert.Count > 0)
                        await storageBroker.BulkInsertAsync(toInsert, cancellationToken);

                    if (toUpdate.Count > 0)
                        await storageBroker.BulkUpdateAsync(toUpdate, cancellationToken);

                    await storageBroker.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
            else
            {
                try
                {
                    var query = await storageBroker.SelectAllAsync<T>();
                    var existingKeys = new HashSet<object>(
                        query.Where(predicate).ToList()
                            .Select(e => keyProperty.PropertyInfo.GetValue(e))
                            .Where(k => k != null)
                            .Cast<object>());

                    var toInsert = objectList
                        .Where(obj =>
                        {
                            var key = keyProperty.PropertyInfo.GetValue(obj);
                            return key == null || !existingKeys.Contains(key);
                        })
                        .ToList();

                    var toUpdate = objectList
                        .Where(obj =>
                        {
                            var key = keyProperty.PropertyInfo.GetValue(obj);
                            return key != null && existingKeys.Contains(key);
                        })
                        .ToList();

                    if (toInsert.Count > 0)
                        await storageBroker.BulkInsertAsync(toInsert, cancellationToken);

                    if (toUpdate.Count > 0)
                        await storageBroker.BulkUpdateAsync(toUpdate, cancellationToken);

                    await storageBroker.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    foreach (var @object in objectList)
                        await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
                }
            }
        }

        public async ValueTask<bool> ExistsAsync<T>(
            object[] objectIds,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(objectIds);
            cancellationToken.ThrowIfCancellationRequested();

            var entityType = await this.storageBroker.FindEntityTypeAsync<T>();
            var keyProperties = entityType?.FindPrimaryKey()?.Properties;

            if (keyProperties == null || keyProperties.Count == 0)
                throw new InvalidOperationException(
                    $"No primary key defined for entity {typeof(T).Name}");

            if (objectIds.Length != keyProperties.Count)
                throw new InvalidOperationException(
                    $"Expected {keyProperties.Count} key value(s) for entity {typeof(T).Name}, "
                    + $"but received {objectIds.Length}.");

            var parameter = Expression.Parameter(typeof(T), "e");
            Expression body = null;

            for (int i = 0; i < keyProperties.Count; i++)
            {
                var keyProperty = keyProperties[i];
                var propAccess = Expression.Property(parameter, keyProperty.Name);
                var typedValue = Convert.ChangeType(objectIds[i], keyProperty.ClrType);
                var equality = Expression.Equal(propAccess, Expression.Constant(typedValue, keyProperty.ClrType));
                body = body is null ? equality : Expression.AndAlso(body, equality);
            }

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            var query = await storageBroker.SelectAllAsync<T>();

            return query.AsNoTracking().Any(predicate);
        }
    }
}
