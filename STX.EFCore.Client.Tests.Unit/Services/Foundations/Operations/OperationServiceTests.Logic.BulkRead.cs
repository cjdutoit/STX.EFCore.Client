// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkReadAsyncShouldReturnAllTheRecords()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            List<User> inputUsers = randomUsers;
            List<User> storageUsers = inputUsers.ToList();
            List<User> expectedUsers = storageUsers.DeepClone();

            storageBrokerMock.Setup(broker =>
               broker.BulkReadAsync(inputUsers))
                   .ReturnsAsync(storageUsers);

            // When
            IEnumerable<User> actualUsers = await operationService.BulkReadAsync(inputUsers);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.BulkReadAsync(inputUsers),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
