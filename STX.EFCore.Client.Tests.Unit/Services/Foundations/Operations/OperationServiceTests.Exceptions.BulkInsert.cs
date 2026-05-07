// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using Moq;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkInsertAsyncShouldRollbackRecordsOnErrorWithTransaction()
        {
            // Given
            bool useTransaction = true;
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> newUsers = randomUsers;
            Exception someException = new Exception(message: GetRandomString());

            var failedOperationServiceException =
                new FailedOperationServiceException(
                    message: "Unexpected operation service error occurred. Contact support.",
                    innerException: someException);

            var expectedOperationServiceException =
                new OperationServiceException(
                    message: "Operation service error occurred, contact support.",
                    innerException: failedOperationServiceException);

            storageBrokerMock.Setup(broker =>
                broker.BeginTransactionAsync(default))
                    .ReturnsAsync(dbContextTransactionMock.Object);

            storageBrokerMock.Setup(broker =>
                broker.BulkInsertAsync(It.IsAny<IEnumerable<User>>(), default))
                    .ThrowsAsync(someException);

            // When
            ValueTask insertUserTask = operationService.BulkInsertAsync(objects: newUsers, useTransaction);

            OperationServiceException actualException =
                await Assert.ThrowsAsync<OperationServiceException>(testCode: insertUserTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationServiceException);

            storageBrokerMock.Verify(broker =>
                broker.BeginTransactionAsync(default),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(newUsers, default),
                    Times.Once);

            dbContextTransactionMock.Verify(transaction =>
                transaction.RollbackAsync(default),
                    Times.Once);

            foreach (var user in newUsers)
            {
                storageBrokerMock.Verify(broker =>
                    broker.UpdateObjectStateAsync(user, EntityState.Detached),
                        Times.Once);
            }

            dbContextTransactionMock.Verify(transaction =>
                transaction.Dispose(),
                    Times.Once);

            dbContextTransactionMock.Verify(transaction =>
                transaction.CommitAsync(default),
                    Times.Never);

            storageBrokerMock.VerifyNoOtherCalls();
            dbContextTransactionMock.VerifyNoOtherCalls();
        }
    }
}
