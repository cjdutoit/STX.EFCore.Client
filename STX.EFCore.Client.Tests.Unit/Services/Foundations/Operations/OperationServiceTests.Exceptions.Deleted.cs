// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task DeleteAsyncShouldDetachEntityWhenExceptionIsThrown()
        {
            // Given
            User randomUser = CreateRandomUser();
            User deleteUser = randomUser;
            User expectedUser = deleteUser.DeepClone();
            Exception errorException = new Exception("Database error");
            Exception expectedException = errorException.DeepClone();

            storageBrokerMock.Setup(broker =>
                broker.UpdateObjectStateAsync(deleteUser, EntityState.Deleted))
                    .ThrowsAsync(errorException);

            // When
            ValueTask<User> insertUserTask = operationService.DeleteAsync(@object: deleteUser);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(testCode: insertUserTask.AsTask);

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(deleteUser, EntityState.Deleted),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(deleteUser, EntityState.Detached),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(),
                    Times.Never);

            storageBrokerMock.VerifyNoOtherCalls();

        }
    }
}
