// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkDeleteAsyncShouldMarkEntityAsAddedSaveChangesAndDetach()
        {
            // Given
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> inputUsers = randomUsers;
            List<EntityState?> statesBeforeSave = new List<EntityState?>();
            List<EntityState?> statesAfterSave = new List<EntityState?>();
            List<EntityState?> statesAfterExplicitDetach = new List<EntityState?>();
            await dbContext.BulkInsertAsync(inputUsers);

            // When
            await operationService.BulkDeleteAsync(inputUsers);

            foreach (var user in inputUsers)
            {
                statesAfterExplicitDetach.Add(dbContext.Entry(user).State);
            }

            // Then
            statesAfterExplicitDetach.Should().AllBeEquivalentTo(EntityState.Detached);

            foreach (var user in inputUsers)
            {
                var userInDatabase = await dbContext.Users.FindAsync(user.Id);
                userInDatabase.Should().BeNull();
            }
        }
    }
}
