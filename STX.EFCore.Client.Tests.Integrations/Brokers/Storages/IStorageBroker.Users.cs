// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STX.EFCore.Client.Tests.Integrations.Models.Users;

namespace STX.EFCore.Client.Tests.Integrations.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<User> InsertUserAsync(User user);
        ValueTask<IQueryable<User>> SelectAllUsersAsync();
        ValueTask<User> SelectUserByIdAsync(Guid userId);
        ValueTask<User> UpdateUserAsync(User user);
        ValueTask<User> DeleteUserAsync(User user);
        ValueTask BulkInsertUsersAsync(IEnumerable<User> users);
        ValueTask<IEnumerable<User>> BulkReadUsersAsync(IEnumerable<User> users);
        ValueTask BulkUpdateUsersAsync(IEnumerable<User> users);
        ValueTask BulkDeleteUsersAsync(IEnumerable<User> users);
    }
}
