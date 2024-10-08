// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task InsertAsyncShouldMarkEntityAsAddedSaveChangesAndDetach()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();

            // When
            User actualUser = await operationService.InsertAsync(@object: inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(inputUser, EntityState.Added),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.UpdateObjectStateAsync(inputUser, EntityState.Detached),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
