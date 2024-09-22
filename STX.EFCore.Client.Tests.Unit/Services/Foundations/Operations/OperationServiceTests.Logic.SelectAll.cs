﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task SelectAllAsyncShouldOnlyReturnExpectedUsersAsync()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            List<User> inputUsers = randomUsers;
            List<User> storageUsers = inputUsers;
            List<User> expectedUsers = storageUsers.DeepClone();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            TestDbContext dbContext = new TestDbContext(options);
            await dbContext.Users.AddRangeAsync(inputUsers);
            await dbContext.SaveChangesAsync();
            var operationService = new OperationService(dbContext);

            // When
            IQueryable<User> actualUsersQuery = await operationService.ReadAllAsync<User>();

            List<User> actualUsers = await actualUsersQuery
                .Where(users => inputUsers.Select(inputUsers => inputUsers.Id)
                    .Contains(users.Id)).ToListAsync();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);
            actualUsers.Count.Should().Be(expectedUsers.Count);
            await dbContext.Database.EnsureDeletedAsync();
        }
    }
}