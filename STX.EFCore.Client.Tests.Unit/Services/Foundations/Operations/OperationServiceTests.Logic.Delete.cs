// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task DeleteAsyncShouldMarkEntityAsDeletedSaveChangesAndDetach()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User deletedUser = inputUser;
            User expectedUser = inputUser.DeepClone();
            EntityState? stateBeforeSave = null;
            EntityState? stateAfterSave = null;
            await dbContext.Users.AddAsync(inputUser);
            await dbContext.SaveChangesAsync();

            dbContext.SavingChanges += (sender, e) =>
            {
                var entry = dbContext.Entry(inputUser);
                stateBeforeSave = entry.State;
            };

            dbContext.SavedChanges += (sender, e) =>
            {
                var entry = dbContext.Entry(inputUser);
                stateAfterSave = entry.State;
            };

            // When
            User actualUser = await operationService.DeleteAsync(inputUser);

            // Then
            stateBeforeSave.Should().Be(EntityState.Deleted);
            stateAfterSave.Should().Be(EntityState.Detached);
            var userInDatabase = await dbContext.Users.FindAsync(deletedUser.Id);
            userInDatabase.Should().BeNull();
            actualUser.Should().BeEquivalentTo(expectedUser);
        }
    }
}
