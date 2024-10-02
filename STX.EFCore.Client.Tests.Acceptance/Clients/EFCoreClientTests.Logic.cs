// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using STX.EFCore.Client.Tests.Acceptance.Models.Users;

namespace STX.EFCore.Client.Tests.Acceptance.Clients
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task ShouldInsertUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();

            // When
            User actualUser = await efCoreClient.InsertAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            if (actualUser != null)
            {
                await efCoreClient.DeleteAsync(actualUser);
            }
        }
    }
}
