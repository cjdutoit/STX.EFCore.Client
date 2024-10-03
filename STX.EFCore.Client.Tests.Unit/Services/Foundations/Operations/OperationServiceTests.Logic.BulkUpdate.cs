// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Force.DeepCloner;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkUpdateAsyncShouldMarkEntityAsAddedSaveChangesAndDetach()
        {
            // Given
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> updatedUsers = randomUsers.DeepClone();

            // When
            await operationService.BulkUpdateAsync(updatedUsers);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.BulkUpdateAsync(updatedUsers),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
