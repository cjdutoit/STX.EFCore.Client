// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkInsertAsyncShouldAddAllTheRecordsWithoutTransaction()
        {
            // Given
            bool useTransaction = false;
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> inputUsers = randomUsers;

            // When
            await operationService.BulkInsertAsync(objects: inputUsers, useTransaction);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(inputUsers),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkInsertAsyncShouldAddAllTheRecordsWhenWithTransaction()
        {
            // Given
            bool useTransaction = true;
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> inputUsers = randomUsers;

            storageBrokerMock.Setup(broker =>
                broker.BeginTransactionAsync())
                    .ReturnsAsync(dbContextTransactionMock.Object);

            // When
            await operationService.BulkInsertAsync(objects: inputUsers, useTransaction);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.BeginTransactionAsync(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(inputUsers),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(),
                    Times.Once);

            dbContextTransactionMock.Verify(transaction =>
                transaction.CommitAsync(default),
                    Times.Once);

            foreach (var user in inputUsers)
            {
                storageBrokerMock.Verify(broker =>
                    broker.UpdateObjectStateAsync(user, EntityState.Detached),
                        Times.Once);
            }

            dbContextTransactionMock.Verify(transaction =>
                transaction.Dispose(),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
            dbContextTransactionMock.VerifyNoOtherCalls();
        }
    }
}
