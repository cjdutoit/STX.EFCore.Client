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
        public async Task UpdateAsyncShouldDetachEntityWhenExceptionIsThrown()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            Exception errorException = new Exception("Database error");
            Exception expectedException = errorException.DeepClone();

            storageBrokerMock.Setup(broker =>
                broker.UpdateObjectStateAsync(inputUser, EntityState.Modified))
                    .ThrowsAsync(errorException);

            // When
            ValueTask<User> insertUserTask = operationService.UpdateAsync(inputUser);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(
                    insertUserTask.AsTask);

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(inputUser, EntityState.Modified),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(inputUser, EntityState.Detached),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(),
                    Times.Never);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
