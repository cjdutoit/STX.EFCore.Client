// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task InsertAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask<User> insertUserTask =
                operationService.InsertAsync(@object: someUser, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: insertUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAllAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask<System.Linq.IQueryable<User>> selectAllTask =
                operationService.SelectAllAsync<User>(cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: selectAllTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            object[] someObjectIds = new object[] { Guid.NewGuid() };
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask<User> selectTask =
                operationService.SelectAsync<User>(
                    objectIds: someObjectIds,
                    cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: selectTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask<User> updateUserTask =
                operationService.UpdateAsync(@object: someUser, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: updateUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask<User> deleteUserTask =
                operationService.DeleteAsync(@object: someUser, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: deleteUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkInsertAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            IEnumerable<User> someUsers = CreateRandomUsers();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask bulkInsertTask =
                operationService.BulkInsertAsync(objects: someUsers, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: bulkInsertTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkReadAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            IEnumerable<User> someUsers = CreateRandomUsers();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask<IEnumerable<User>> bulkReadTask =
                operationService.BulkReadAsync(objects: someUsers, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: bulkReadTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpdateAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            IEnumerable<User> someUsers = CreateRandomUsers();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask bulkUpdateTask =
                operationService.BulkUpdateAsync(objects: someUsers, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: bulkUpdateTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkDeleteAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            IEnumerable<User> someUsers = CreateRandomUsers();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // When
            ValueTask bulkDeleteTask =
                operationService.BulkDeleteAsync(objects: someUsers, cancellationToken: cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: bulkDeleteTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
