// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Tests.Acceptance.Models.Users;

namespace STX.EFCore.Client.Tests.Acceptance.Brokers.Storages
{
    public partial class TestDbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
