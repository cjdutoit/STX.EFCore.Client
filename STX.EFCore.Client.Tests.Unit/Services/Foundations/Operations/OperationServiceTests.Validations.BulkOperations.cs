// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkInsertAsyncShouldThrowArgumentNullExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            // When
            ValueTask bulkInsertTask = operationService.BulkInsertAsync(objects: nullUsers);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: bulkInsertTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkReadAsyncShouldThrowArgumentNullExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            // When
            ValueTask<IEnumerable<User>> bulkReadTask = operationService.BulkReadAsync(objects: nullUsers);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: bulkReadTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpdateAsyncShouldThrowArgumentNullExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            // When
            ValueTask bulkUpdateTask = operationService.BulkUpdateAsync(objects: nullUsers);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: bulkUpdateTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkDeleteAsyncShouldThrowArgumentNullExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            // When
            ValueTask bulkDeleteTask = operationService.BulkDeleteAsync(objects: nullUsers);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: bulkDeleteTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpsertAsyncShouldThrowArgumentNullExceptionWhenCollectionIsNull()
        {
            // Given
            IEnumerable<User> nullUsers = null;

            // When
            ValueTask bulkUpsertTask = operationService.BulkUpsertAsync(objects: nullUsers);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: bulkUpsertTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExistsAsyncShouldThrowArgumentNullExceptionWhenObjectIdsIsNull()
        {
            // Given
            object[] nullObjectIds = null;

            // When
            ValueTask<bool> existsTask = operationService.ExistsAsync<User>(objectIds: nullObjectIds);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: existsTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}


