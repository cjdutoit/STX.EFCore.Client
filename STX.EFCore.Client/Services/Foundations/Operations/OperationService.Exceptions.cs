// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;
using Xeptions;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    internal partial class OperationService
    {
        private delegate ValueTask<T> ReturningObjectFunction<T>();
        private delegate ValueTask<IEnumerable<T>> ReturningCollectionFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(ReturningObjectFunction<T> returningObjectFunction)
        {
            try
            {
                return await returningObjectFunction();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullOperationObjectException nullOperationObjectException)
            {
                throw CreateAndLogValidationException(nullOperationObjectException);
            }
            catch (NullOperationObjectIdsException nullOperationObjectIdsException)
            {
                throw CreateAndLogValidationException(nullOperationObjectIdsException);
            }
            catch (NullOperationCollectionException nullOperationCollectionException)
            {
                throw CreateAndLogValidationException(nullOperationCollectionException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationStorageException =
                    new FailedOperationStorageException(
                        message: "Failed operation storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw CreateAndLogDependencyException(failedOperationStorageException);
            }
            catch (Exception serviceException)
            {
                var failedOperationServiceException =
                    new FailedOperationServiceException(
                        message: "Unexpected operation service error occurred. Contact support.",
                        innerException: serviceException);

                throw CreateAndLogServiceException(failedOperationServiceException);
            }
        }

        private async ValueTask<IEnumerable<T>> TryCatch<T>(ReturningCollectionFunction<T> returningCollectionFunction)
        {
            try
            {
                return await returningCollectionFunction();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullOperationCollectionException nullOperationCollectionException)
            {
                throw CreateAndLogValidationException(nullOperationCollectionException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationStorageException =
                    new FailedOperationStorageException(
                        message: "Failed operation storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw CreateAndLogDependencyException(failedOperationStorageException);
            }
            catch (Exception serviceException)
            {
                var failedOperationServiceException =
                    new FailedOperationServiceException(
                        message: "Unexpected operation service error occurred. Contact support.",
                        innerException: serviceException);

                throw CreateAndLogServiceException(failedOperationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullOperationCollectionException nullOperationCollectionException)
            {
                throw CreateAndLogValidationException(nullOperationCollectionException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationStorageException =
                    new FailedOperationStorageException(
                        message: "Failed operation storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw CreateAndLogDependencyException(failedOperationStorageException);
            }
            catch (Exception serviceException)
            {
                var failedOperationServiceException =
                    new FailedOperationServiceException(
                        message: "Unexpected operation service error occurred. Contact support.",
                        innerException: serviceException);

                throw CreateAndLogServiceException(failedOperationServiceException);
            }
        }

        private static OperationValidationException CreateAndLogValidationException(Xeption exception)
        {
            var operationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: exception);

            return operationValidationException;
        }

        private static OperationDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var operationDependencyException =
                new OperationDependencyException(
                    message: "Operation dependency error occurred, contact support.",
                    innerException: exception);

            return operationDependencyException;
        }

        private static OperationServiceException CreateAndLogServiceException(Xeption exception)
        {
            var operationServiceException =
                new OperationServiceException(
                    message: "Operation service error occurred, contact support.",
                    innerException: exception);

            return operationServiceException;
        }
    }
}
