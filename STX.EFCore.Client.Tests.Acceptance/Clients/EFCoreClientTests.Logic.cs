// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Tests.Acceptance.Models.Users;

namespace STX.EFCore.Client.Tests.Acceptance.Clients
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task ShouldInsertUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();

            // When
            User actualUser = await efCoreClient.InsertAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            if (actualUser != null)
            {
                await efCoreClient.DeleteAsync(actualUser);
            }
        }

        [Fact]
        public async Task ShouldSelectAllUsersAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await efCoreClient.InsertAsync(inputUser);

            // When
            IQueryable<User> actualUsers = await efCoreClient.SelectAllAsync<User>();
            User actualUser = await actualUsers.FirstOrDefaultAsync(user => user.Id == inputUser.Id);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            if (actualUser != null)
            {
                await efCoreClient.DeleteAsync(actualUser);
            }
        }

        [Fact]
        public async Task ShouldSelectUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await efCoreClient.InsertAsync(inputUser);

            // When
            User actualUser = await efCoreClient.SelectAsync<User>(inputUser.Id);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            if (actualUser != null)
            {
                await efCoreClient.DeleteAsync(actualUser);
            }
        }

        [Fact]
        public async Task ShouldDeleteUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await efCoreClient.InsertAsync(inputUser);

            // When
            User actualUser = await efCoreClient.DeleteAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);
            User userInDatabase = await efCoreClient.SelectAsync<User>(inputUser.Id);
            userInDatabase.Should().BeNull();
        }

        [Fact]
        public async Task ShouldBulkInsertUsersAsync()
        {
            // Given
            int numberOfUsers = GetRandomNumber();
            List<User> randomUsers = CreateRandomUsers(count: numberOfUsers);
            List<User> inputUsers = randomUsers;
            List<User> expectedUsers = inputUsers.DeepClone();
            List<Guid> expectedUserIds = expectedUsers.Select(u => u.Id).ToList();

            // When
            await efCoreClient.BulkInsertAsync<User>(inputUsers);
            IQueryable<User> users = await efCoreClient.SelectAllAsync<User>();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            foreach (User user in inputUsers)
            {
                await efCoreClient.DeleteAsync(user);
            }
        }

        [Fact]
        public async Task ShouldBulkReadUsersAsync()
        {
            // Given
            int numberOfUsers = GetRandomNumber();
            List<User> randomUsers = CreateRandomUsers(count: numberOfUsers);
            List<User> inputUsers = randomUsers;
            List<User> updatedUsers = inputUsers.DeepClone();
            updatedUsers.ForEach(user => user.Email = GetRandomString());
            List<User> expectedUsers = updatedUsers.DeepClone();
            List<Guid> expectedUserIds = expectedUsers.Select(u => u.Id).ToList();
            await efCoreClient.BulkInsertAsync(inputUsers);

            // When
            await efCoreClient.BulkReadUsersAsync(updatedUsers);
            IQueryable<User> users = await efCoreClient.SelectAllAsync<User>();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            foreach (User user in inputUsers)
            {
                await efCoreClient.DeleteAsync(user);
            }
        }

        [Fact]
        public async Task ShouldBulkUpdateUsersAsync()
        {
            // Given
            int numberOfUsers = GetRandomNumber();
            List<User> randomUsers = CreateRandomUsers(count: numberOfUsers);
            List<User> inputUsers = randomUsers;
            List<User> updatedUsers = inputUsers.DeepClone();
            updatedUsers.ForEach(user => user.Email = GetRandomString());
            List<User> expectedUsers = updatedUsers.DeepClone();
            List<Guid> expectedUserIds = expectedUsers.Select(u => u.Id).ToList();
            await efCoreClient.BulkInsertAsync(inputUsers);

            // When
            await efCoreClient.BulkUpdateAsync(updatedUsers);
            IQueryable<User> users = await efCoreClient.SelectAllAsync<User>();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            foreach (User user in inputUsers)
            {
                await efCoreClient.DeleteAsync(user);
            }
        }

        [Fact]
        public async Task ShouldBulkDeleteUsersAsync()
        {
            // Given
            int numberOfUsers = GetRandomNumber();
            List<User> randomUsers = CreateRandomUsers(count: numberOfUsers);
            List<User> inputUsers = randomUsers;
            List<User> updatedUsers = inputUsers.DeepClone();
            updatedUsers.ForEach(user => user.Email = GetRandomString());
            List<User> expectedUsers = updatedUsers.DeepClone();
            List<Guid> expectedUserIds = expectedUsers.Select(u => u.Id).ToList();
            await efCoreClient.BulkInsertAsync(inputUsers);

            // When
            await efCoreClient.BulkDeleteAsync(updatedUsers);


            // Then
            IQueryable<User> users = await efCoreClient.SelectAllAsync<User>();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            actualUsers.Should().HaveCount(0);
        }
    }
}
