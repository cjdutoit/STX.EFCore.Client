// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task SelectAsyncShouldReturnExpectedObjectAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User storageUser = randomUser;
            User expectedUser = storageUser.DeepClone();
            object[] objectIds = new object[] { randomUser.Id };

            operationServiceMock.Setup(service =>
                service.SelectAsync<User>(objectIds, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageUser);

            // When
            User actualUser = await efCoreClient.SelectAsync<User>(objectIds);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            operationServiceMock.Verify(service =>
                service.SelectAsync<User>(objectIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
