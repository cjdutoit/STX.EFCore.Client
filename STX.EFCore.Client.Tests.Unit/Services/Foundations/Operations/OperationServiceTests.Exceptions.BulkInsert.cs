﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using Moq;
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
            Exception expectedException = someException.DeepClone();

            storageBrokerMock.Setup(broker =>
                broker.BeginTransactionAsync())
                    .ReturnsAsync(dbContextTransactionMock.Object);

            storageBrokerMock.Setup(broker =>
                broker.BulkInsertAsync(It.IsAny<IEnumerable<User>>()))
                    .ThrowsAsync(someException);

            // When
            ValueTask insertUserTask = operationService.BulkInsertAsync(objects: newUsers, useTransaction);
            Exception actualException = await Assert.ThrowsAsync<Exception>(testCode: insertUserTask.AsTask);

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);

            storageBrokerMock.Verify(broker =>
                broker.BeginTransactionAsync(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(newUsers),
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
