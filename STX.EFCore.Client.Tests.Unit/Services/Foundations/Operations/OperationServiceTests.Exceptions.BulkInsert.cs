// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task BulkInsertAsyncShouldDetachAllEntitiesWhenExceptionIsThrown()
        {
            // Given
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> inputUsers = randomUsers;
            List<EntityState?> statesAfterExplicitDetach = new List<EntityState?>();
            Exception errorException = new Exception("Database error");
            Exception expectedException = errorException.DeepClone();
            bool firstTime = true;

            dbContext.SavingChanges += (sender, e) =>
            {
                if (firstTime)
                {
                    firstTime = false;
                    throw errorException;
                }
            };

            // When
            ValueTask bulkInsertUserTask = operationService.BulkInsertAsync(inputUsers);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(
                    bulkInsertUserTask.AsTask);

            foreach (var user in inputUsers)
            {
                statesAfterExplicitDetach.Add(dbContext.Entry(user).State);
            }

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);
            statesAfterExplicitDetach.Should().AllBeEquivalentTo(EntityState.Detached);
            await dbContext.BulkDeleteAsync(inputUsers);
        }
    }
}
