// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task InsertAsyncShouldThrowArgumentNullExceptionWhenObjectIsNull()
        {
            // Given
            User nullUser = null;

            // When
            ValueTask<User> insertUserTask = operationService.InsertAsync(@object: nullUser);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: insertUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsyncShouldThrowArgumentNullExceptionWhenObjectIsNull()
        {
            // Given
            User nullUser = null;

            // When
            ValueTask<User> updateUserTask = operationService.UpdateAsync(@object: nullUser);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: updateUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowArgumentNullExceptionWhenObjectIsNull()
        {
            // Given
            User nullUser = null;

            // When
            ValueTask<User> deleteUserTask = operationService.DeleteAsync(@object: nullUser);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: deleteUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldThrowArgumentNullExceptionWhenObjectIdsIsNull()
        {
            // Given
            object[] nullObjectIds = null;

            // When
            ValueTask<User> selectUserTask = operationService.SelectAsync<User>(objectIds: nullObjectIds);

            // Then
            await Assert.ThrowsAsync<ArgumentNullException>(testCode: selectUserTask.AsTask);
            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
