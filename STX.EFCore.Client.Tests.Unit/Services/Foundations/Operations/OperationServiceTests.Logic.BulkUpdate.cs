// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Services.Foundations.Operations;
using STX.EFCore.Client.Tests.Unit.Brokers.Storages;
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
            IEnumerable<User> inputUsers = randomUsers;
            List<EntityState?> statesBeforeSave = new List<EntityState?>();
            List<EntityState?> statesAfterSave = new List<EntityState?>();
            List<EntityState?> statesAfterExplicitDetach = new List<EntityState?>();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            TestDbContext dbContext = new TestDbContext(options);
            OperationService operationService = new OperationService(dbContext);

            foreach (var user in inputUsers)
            {
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            dbContext.SavingChanges += (sender, e) =>
            {
                foreach (var user in inputUsers)
                {
                    var entry = dbContext.Entry(user);
                    statesBeforeSave.Add(entry.State);
                }
            };

            dbContext.SavedChanges += (sender, e) =>
            {
                foreach (var user in inputUsers)
                {
                    var entry = dbContext.Entry(user);
                    statesAfterSave.Add(entry.State);
                }
            };

            // When
            await operationService.BulkUpdateAsync(inputUsers);

            foreach (var user in inputUsers)
            {
                statesAfterExplicitDetach.Add(dbContext.Entry(user).State);
            }

            // Then
            statesBeforeSave.Should().AllBeEquivalentTo(EntityState.Modified);
            statesAfterSave.Should().AllBeEquivalentTo(EntityState.Unchanged);
            statesAfterExplicitDetach.Should().AllBeEquivalentTo(EntityState.Detached);

            foreach (var user in inputUsers)
            {
                var userInDatabase = await dbContext.Users.FindAsync(user.Id);
                userInDatabase.Should().NotBeNull();

                if (userInDatabase != null)
                {
                    dbContext.Users.Remove(userInDatabase);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
