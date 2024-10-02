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
        public async Task BulkInsertAsyncShouldMarkEntityAsAddedSaveChangesAndDetach()
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

            // When
            await operationService.BulkInsertAsync(inputUsers);

            foreach (var user in inputUsers)
            {
                statesAfterExplicitDetach.Add(dbContext.Entry(user).State);
            }

            // Then
            statesAfterExplicitDetach.Should().AllBeEquivalentTo(EntityState.Detached);
            await dbContext.BulkDeleteAsync(inputUsers);
        }
    }
}
