// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task UpdateAsyncShouldDetachEntityWhenExceptionIsThrown()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            EntityState? stateBeforeSave = null;
            Exception errorException = new Exception("Database error");
            Exception expectedException = errorException.DeepClone();

            dbContext.SavingChanges += (sender, e) =>
            {
                var entry = dbContext.Entry(inputUser);
                stateBeforeSave = entry.State;
                throw errorException;
            };

            // When
            ValueTask<User> insertUserTask = operationService.UpdateAsync(inputUser);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(
                    insertUserTask.AsTask);

            EntityState stateAfterExplicitDetach = dbContext.Entry(inputUser).State;

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);
            stateBeforeSave.Should().Be(EntityState.Modified);
            stateAfterExplicitDetach.Should().Be(EntityState.Detached);
            var userInDatabase = await dbContext.Users.FindAsync(inputUser.Id);

            if (userInDatabase != null)
            {
                dbContext.Users.Remove(userInDatabase);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
