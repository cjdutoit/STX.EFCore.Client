// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task SelectAsyncShouldReturnExpectedUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await dbContext.Users.AddAsync(inputUser);
            await dbContext.SaveChangesAsync();

            // When
            User actualUser = await operationService.SelectAsync<User>(inputUser.Id);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);
            var userInDatabase = await dbContext.Users.FindAsync(inputUser.Id);
            dbContext.Users.Remove(userInDatabase);
            await dbContext.SaveChangesAsync();
        }
    }
}
