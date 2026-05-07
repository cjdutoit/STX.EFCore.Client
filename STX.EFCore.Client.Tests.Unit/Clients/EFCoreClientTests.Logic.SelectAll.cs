// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task SelectAllAsyncShouldReturnExpectedUsersAsync()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            IQueryable<User> expectedUsers = randomUsers.AsQueryable().DeepClone();

            operationServiceMock.Setup(service =>
                service.SelectAllAsync<User>(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomUsers.AsQueryable());

            // When
            IQueryable<User> actualUsers = await efCoreClient.SelectAllAsync<User>();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            operationServiceMock.Verify(service =>
                service.SelectAllAsync<User>(It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
