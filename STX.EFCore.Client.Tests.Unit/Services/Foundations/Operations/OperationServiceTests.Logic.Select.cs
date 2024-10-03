// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task SelectAsyncShouldReturnExpectedUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User storageUser = inputUser;
            User expectedUser = storageUser.DeepClone();

            storageBrokerMock.Setup(broker =>
                broker.SelectAsync<User>(inputUser.Id))
                    .ReturnsAsync(storageUser);

            // When
            User actualUser = await operationService.SelectAsync<User>(inputUser.Id);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            storageBrokerMock.Verify(broker =>
                broker.SelectAsync<User>(inputUser.Id),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
