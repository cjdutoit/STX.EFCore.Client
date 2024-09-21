// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Services.Foundations;
using STX.EFCore.Client.Tests.Unit.Brokers.Storages;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task InsertAsyncShouldMarkEntityAsAddedSaveChangesAndDetach()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            EntityState? stateBeforeSave = null;
            EntityState? stateAfterSave = null;

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            TestDbContext dbContext = new TestDbContext(options);
            OperationService operationService = new OperationService(dbContext);

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
            User actualUser = await operationService.InsertAsync(inputUser);
            EntityState stateAfterExplicitDetach = dbContext.Entry(inputUser).State;

            // Then
            stateBeforeSave.Should().Be(EntityState.Added);
            stateAfterSave.Should().Be(EntityState.Unchanged);
            stateAfterExplicitDetach.Should().Be(EntityState.Detached);
            var userInDatabase = await dbContext.Users.FindAsync(inputUser.Id);
            userInDatabase.Should().NotBeNull();
            actualUser.Should().BeEquivalentTo(expectedUser);
            await dbContext.Database.EnsureDeletedAsync();
        }
    }
}
