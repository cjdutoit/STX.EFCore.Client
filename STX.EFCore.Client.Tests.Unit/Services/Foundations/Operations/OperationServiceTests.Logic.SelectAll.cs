// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task SelectAllAsyncShouldOnlyReturnExpectedUsersAsync()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            IQueryable<User> inputUsers = randomUsers.AsQueryable();
            IQueryable<User> storageUsers = inputUsers;
            IQueryable<User> expectedUsers = storageUsers.DeepClone();

            storageBrokerMock.Setup(broker =>
                broker.SelectAllAsync<User>())
                    .ReturnsAsync(storageUsers.AsQueryable());

            // When
            IQueryable<User> actualUsers = await operationService.SelectAllAsync<User>();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
