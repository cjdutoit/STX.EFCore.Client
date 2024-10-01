// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task BulkUpdateAsyncShouldDetachAllEntitiesWhenExceptionIsThrown()
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
            Exception errorException = new Exception("Database error");
            Exception expectedException = errorException.DeepClone();

            dbContext.SavingChanges += (sender, e) =>
            {
                throw errorException;
            };

            // When
            ValueTask bulkUpdateUserTask = operationService.BulkUpdateAsync(inputUsers);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(
                    bulkUpdateUserTask.AsTask);

            foreach (var user in inputUsers)
            {
                statesAfterExplicitDetach.Add(dbContext.Entry(user).State);
            }

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);
            statesAfterExplicitDetach.Should().AllBeEquivalentTo(EntityState.Detached);

            foreach (var user in inputUsers)
            {
                var userInDatabase = await dbContext.Users.FindAsync(user.Id);

                if (userInDatabase != null)
                {
                    dbContext.Users.Remove(userInDatabase);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
