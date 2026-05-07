// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Clients
{
    public partial class EFCoreClientTests
    {
        [Fact]
        public async Task BulkUpsertAsyncShouldDelegateToOperationServiceAsync()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            List<User> inputUsers = randomUsers;

            // When
            await efCoreClient.BulkUpsertAsync(inputUsers);

            // Then
            operationServiceMock.Verify(service =>
                service.BulkUpsertAsync(inputUsers, true, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
