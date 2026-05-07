// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Clients
{
    public partial class EFCoreClientTests
    {
        [Fact]
        public async Task UpdateAsyncShouldReturnUpdatedObjectAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();

            operationServiceMock.Setup(service =>
                service.UpdateAsync(inputUser, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(inputUser);

            // When
            User actualUser = await efCoreClient.UpdateAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            operationServiceMock.Verify(service =>
                service.UpdateAsync(inputUser, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
