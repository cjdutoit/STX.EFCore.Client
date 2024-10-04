// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STX.EFCore.Client.Tests.Acceptance.Models.Users;

namespace STX.EFCore.Client.Tests.Acceptance.Brokers.Storages
{
    public partial class TestDbContext
    {
        private static void AddUserConfigurations(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
        }
    }
}
