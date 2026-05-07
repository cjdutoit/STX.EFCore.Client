// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task DeleteAsyncShouldDetachEntityWhenExceptionIsThrown()
        {
            // Given
            User randomUser = CreateRandomUser();
            User deleteUser = randomUser;
            User expectedUser = deleteUser.DeepClone();
            Exception errorException = new Exception("Database error");

            var failedOperationServiceException =
                new FailedOperationServiceException(
                    message: "Unexpected operation service error occurred. Contact support.",
                    innerException: errorException);

            var expectedOperationServiceException =
                new OperationServiceException(
                    message: "Operation service error occurred, contact support.",
                    innerException: failedOperationServiceException);

            storageBrokerMock.Setup(broker =>
                broker.UpdateObjectStateAsync(deleteUser, EntityState.Deleted))
                    .ThrowsAsync(errorException);

            // When
            ValueTask<User> deleteUserTask = operationService.DeleteAsync(@object: deleteUser);

            OperationServiceException actualException =
                await Assert.ThrowsAsync<OperationServiceException>(testCode: deleteUserTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedOperationServiceException);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(deleteUser, EntityState.Deleted),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(deleteUser, EntityState.Detached),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(default),
                    Times.Never);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
