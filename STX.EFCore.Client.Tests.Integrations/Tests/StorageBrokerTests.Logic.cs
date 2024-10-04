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
using STX.EFCore.Client.Tests.Integrations.Models.Users;

namespace STX.EFCore.Client.Tests.Integrations.Tests
{
    public partial class StorageBrokerTests
    {
        [Fact]
        public async Task ShouldInsertUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();

            // When
            User actualUser = await storageBroker.InsertUserAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);
            await storageBroker.DeleteUserAsync(actualUser);
        }


        [Fact]
        public async Task ShouldSelectAllUsersAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await storageBroker.InsertUserAsync(inputUser);

            // When
            IQueryable<User> actualUsers = await storageBroker.SelectAllUsersAsync();
            User actualUser = await actualUsers.FirstOrDefaultAsync(user => user.Id == inputUser.Id);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);
            await storageBroker.DeleteUserAsync(actualUser);
        }

        [Fact]
        public async Task ShouldSelectUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await storageBroker.InsertUserAsync(inputUser);

            // When
            User actualUser = await storageBroker.SelectUserByIdAsync(inputUser.Id);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);
            await storageBroker.DeleteUserAsync(actualUser);
        }



        [Fact]
        public async Task ShouldDeleteUserAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();
            await storageBroker.InsertUserAsync(inputUser);

            // When
            User actualUser = await storageBroker.DeleteUserAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);
            User userInDatabase = await storageBroker.SelectUserByIdAsync(inputUser.Id);
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
            await storageBroker.BulkInsertUsersAsync(inputUsers);
            IQueryable<User> users = await storageBroker.SelectAllUsersAsync();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);
            await storageBroker.BulkDeleteUsersAsync(actualUsers);
        }

        [Fact]
        public async Task ShouldBulkReadUsersAsync()
        {
            // Given
            int numberOfUsers = GetRandomNumber();
            IEnumerable<User> randomUsers = CreateRandomUsers(count: numberOfUsers);
            IEnumerable<User> inputUsers = randomUsers;
            IEnumerable<User> expectedUsers = inputUsers.DeepClone();
            IEnumerable<Guid> expectedUserIds = expectedUsers.Select(u => u.Id).ToList();
            await storageBroker.BulkInsertUsersAsync(inputUsers);

            // When
            IEnumerable<User> actualUsers = await storageBroker.BulkReadUsersAsync(inputUsers);

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);
            await storageBroker.BulkDeleteUsersAsync(actualUsers);
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
            await storageBroker.BulkInsertUsersAsync(inputUsers);

            // When
            await storageBroker.BulkUpdateUsersAsync(updatedUsers);
            IQueryable<User> users = await storageBroker.SelectAllUsersAsync();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);
            await storageBroker.BulkDeleteUsersAsync(actualUsers);
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
            await storageBroker.BulkInsertUsersAsync(inputUsers);

            // When
            await storageBroker.BulkDeleteUsersAsync(updatedUsers);

            // Then
            IQueryable<User> users = await storageBroker.SelectAllUsersAsync();

            List<User> actualUsers = await users
                .Where(u => expectedUserIds.Contains(u.Id)).ToListAsync();

            actualUsers.Should().HaveCount(0);
        }
    }
}
