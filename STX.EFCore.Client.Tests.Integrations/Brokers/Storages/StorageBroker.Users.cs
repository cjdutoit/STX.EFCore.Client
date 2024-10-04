// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Tests.Integrations.Models.Users;

namespace STX.EFCore.Client.Tests.Integrations.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);

        public async ValueTask<IQueryable<User>> SelectAllUsersAsync() =>
            await SelectAllAsync<User>();

        public async ValueTask<User> SelectUserByIdAsync(Guid userId) =>
            await SelectAsync<User>(userId);

        public async ValueTask<User> UpdateUserAsync(User user) =>
            await UpdateAsync(user);

        public async ValueTask<User> DeleteUserAsync(User user) =>
            await DeleteAsync(user);

        public async ValueTask BulkInsertUsersAsync(IEnumerable<User> users) =>
            await efCoreClient.BulkInsertAsync<User>(users);

        public async ValueTask<IEnumerable<User>> BulkReadUsersAsync(IEnumerable<User> users) =>
            await efCoreClient.BulkReadAsync<User>(users);

        public async ValueTask BulkUpdateUsersAsync(IEnumerable<User> users) =>
            await efCoreClient.BulkUpdateAsync<User>(users);

        public async ValueTask BulkDeleteUsersAsync(IEnumerable<User> users) =>
            await efCoreClient.BulkDeleteAsync<User>(users);
    }
}
