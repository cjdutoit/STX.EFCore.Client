// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkInsertAsyncShouldThrowValidationExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            var nullOperationCollectionException =
                new NullOperationCollectionException(
                    message: "Operation collection is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationCollectionException);

            // When
            ValueTask bulkInsertTask = operationService.BulkInsertAsync(objects: nullUsers);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: bulkInsertTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkReadAsyncShouldThrowValidationExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            var nullOperationCollectionException =
                new NullOperationCollectionException(
                    message: "Operation collection is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationCollectionException);

            // When
            ValueTask<IEnumerable<User>> bulkReadTask = operationService.BulkReadAsync(objects: nullUsers);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: bulkReadTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpdateAsyncShouldThrowValidationExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            var nullOperationCollectionException =
                new NullOperationCollectionException(
                    message: "Operation collection is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationCollectionException);

            // When
            ValueTask bulkUpdateTask = operationService.BulkUpdateAsync(objects: nullUsers);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: bulkUpdateTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkDeleteAsyncShouldThrowValidationExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            var nullOperationCollectionException =
                new NullOperationCollectionException(
                    message: "Operation collection is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationCollectionException);

            // When
            ValueTask bulkDeleteTask = operationService.BulkDeleteAsync(objects: nullUsers);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: bulkDeleteTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
