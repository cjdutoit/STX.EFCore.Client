// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using STX.EFCore.Client.Brokers.Storages;
using STX.EFCore.Client.Models.Clients.Exceptions;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;
using STX.EFCore.Client.Services.Foundations.Operations;
using Xeptions;

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

        internal EFCoreClient(IOperationService operationService) =>
            this.operationService = operationService;

        public async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await this.operationService.InsertAsync(@object, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await this.operationService.SelectAllAsync<T>(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await this.operationService.SelectAsync<T>(objectIds, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await this.operationService.UpdateAsync(@object, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await this.operationService.DeleteAsync(@object, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask BulkInsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                await this.operationService.BulkInsertAsync(objects, useTransaction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask<IEnumerable<T>> BulkReadAsync<T>(
            IEnumerable<T> objects,
            CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return await this.operationService.BulkReadAsync(objects, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask BulkUpdateAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                await this.operationService.BulkUpdateAsync(objects, useTransaction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask BulkDeleteAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                await this.operationService.BulkDeleteAsync(objects, useTransaction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (OperationValidationException operationValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyValidationException operationDependencyValidationException)
            {
                throw CreateEFCoreClientValidationException(
                    operationDependencyValidationException.InnerException as Xeption);
            }
            catch (OperationDependencyException operationDependencyException)
            {
                throw CreateEFCoreClientDependencyException(
                    operationDependencyException.InnerException as Xeption);
            }
            catch (OperationServiceException operationServiceException)
            {
                throw CreateEFCoreClientServiceException(
                    operationServiceException.InnerException as Xeption);
            }
        }

        private static EFCoreClientValidationException CreateEFCoreClientValidationException(
            Xeption innerException)
        {
            return new EFCoreClientValidationException(
                message: "EFCore client validation error occurred, fix the errors and try again.",
                innerException);
        }

        private static EFCoreClientDependencyException CreateEFCoreClientDependencyException(
            Xeption innerException)
        {
            return new EFCoreClientDependencyException(
                message: "EFCore client dependency error occurred, contact support.",
                innerException);
        }

        private static EFCoreClientServiceException CreateEFCoreClientServiceException(
            Xeption innerException)
        {
            return new EFCoreClientServiceException(
                message: "EFCore client service error occurred, contact support.",
                innerException);
        }

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
