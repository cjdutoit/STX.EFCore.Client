// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Clients
{
    public partial class EFCoreClientTests
    {
        [Fact]
        public async Task ExistsAsyncShouldDelegateToOperationServiceAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            object[] inputIds = new object[] { randomUser.Id };
            bool expectedResult = true;

            operationServiceMock.Setup(service =>
                service.ExistsAsync<User>(inputIds, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedResult);

            // When
            bool actualResult = await efCoreClient.ExistsAsync<User>(inputIds);

            // Then
            actualResult.Should().Be(expectedResult);

            operationServiceMock.Verify(service =>
                service.ExistsAsync<User>(inputIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
