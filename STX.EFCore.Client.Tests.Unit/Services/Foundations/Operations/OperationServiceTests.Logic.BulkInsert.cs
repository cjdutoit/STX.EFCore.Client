// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkInsertAsyncShoulAddAllTheRecords()
        {
            // Given
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> inputUsers = randomUsers;

            // When
            await operationService.BulkInsertAsync(inputUsers);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(inputUsers),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
