// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Brokers.Storages;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    internal class OperationService : IOperationService
    {
        private readonly IStorageBroker storageBroker;

        public OperationService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default) where T
            : class
        {
            try
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Added);
                await storageBroker.SaveChangesAsync(cancellationToken);

                return @object;
            }
            catch
            {
                throw;
            }
            finally
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
            }
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default) where T
            : class =>
            await storageBroker.SelectAllAsync<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await storageBroker.SelectAsync<T>(objectIds);

        public async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken) where T : class =>
            await storageBroker.SelectAsync<T>(objectIds, cancellationToken);

        public async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Modified);
                await storageBroker.SaveChangesAsync(cancellationToken);

                return @object;
            }
            catch
            {
                throw;
            }
            finally
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
            }
        }

        public async ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Deleted);
                await storageBroker.SaveChangesAsync(cancellationToken);

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await storageBroker.UpdateObjectStateAsync(@object, EntityState.Detached);
            }
        }

        public async ValueTask BulkInsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default) where T : class
        {
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
                catch
                {
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
        }

        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(
            IEnumerable<T> objects,
            CancellationToken cancellationToken = default) where T : class
        {
            var entityType = await this.storageBroker.FindEntityTypeAsync<T>();
            var keyProperty = entityType?.FindPrimaryKey()?.Properties?.FirstOrDefault();

            if (keyProperty == null)
            {
                throw new InvalidOperationException($"No primary key defined for entity {typeof(T).Name}");
            }

            var keyType = keyProperty.ClrType;

            var keyValues = objects
                .Select(obj => keyProperty.PropertyInfo.GetValue(obj))
                .Where(key => key != null)
                .ToList();

            var castedKeyValues = typeof(Enumerable)
                .GetMethod(name: "Cast", bindingAttr: BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(typeArguments: keyType)
                .Invoke(obj: null, parameters: new object[] { keyValues });

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

            return query.Where(predicate).ToList();
        }


        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects, bool useTransaction = true, CancellationToken cancellationToken = default) where T : class
        {
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
                catch
                {
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
        }

        public async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects, bool useTransaction = true, CancellationToken cancellationToken = default) where T : class
        {
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
                catch
                {
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
        }
    }
}
