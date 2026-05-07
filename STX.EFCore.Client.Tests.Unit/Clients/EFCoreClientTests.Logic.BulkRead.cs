// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
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
        public async Task BulkReadAsyncShouldReturnExpectedUsersAsync()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            List<User> inputUsers = randomUsers;
            IEnumerable<User> storageUsers = inputUsers.DeepClone();
            IEnumerable<User> expectedUsers = storageUsers.DeepClone();

            operationServiceMock.Setup(service =>
                service.BulkReadAsync(inputUsers, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageUsers);

            // When
            IEnumerable<User> actualUsers = await efCoreClient.BulkReadAsync(inputUsers);

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            operationServiceMock.Verify(service =>
                service.BulkReadAsync(inputUsers, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
