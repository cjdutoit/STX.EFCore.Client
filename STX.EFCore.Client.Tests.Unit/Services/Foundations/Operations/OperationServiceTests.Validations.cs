// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task InsertAsyncShouldThrowValidationExceptionWhenObjectIsNull()
        {
            // Given
            User nullUser = null;

            var nullOperationObjectException =
                new NullOperationObjectException(
                    message: "Operation object is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationObjectException);

            // When
            ValueTask<User> insertUserTask = operationService.InsertAsync(@object: nullUser);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: insertUserTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsyncShouldThrowValidationExceptionWhenObjectIsNull()
        {
            // Given
            User nullUser = null;

            var nullOperationObjectException =
                new NullOperationObjectException(
                    message: "Operation object is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationObjectException);

            // When
            ValueTask<User> updateUserTask = operationService.UpdateAsync(@object: nullUser);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: updateUserTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowValidationExceptionWhenObjectIsNull()
        {
            // Given
            User nullUser = null;

            var nullOperationObjectException =
                new NullOperationObjectException(
                    message: "Operation object is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationObjectException);

            // When
            ValueTask<User> deleteUserTask = operationService.DeleteAsync(@object: nullUser);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: deleteUserTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldThrowValidationExceptionWhenObjectIdsIsNull()
        {
            // Given
            object[] nullObjectIds = null;

            var nullOperationObjectIdsException =
                new NullOperationObjectIdsException(
                    message: "Operation object ids is null, please fix and try again.");

            var expectedOperationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: nullOperationObjectIdsException);

            // When
            ValueTask<User> selectUserTask = operationService.SelectAsync<User>(objectIds: nullObjectIds);

            OperationValidationException actualException =
                await Assert.ThrowsAsync<OperationValidationException>(testCode: selectUserTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationValidationException);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
