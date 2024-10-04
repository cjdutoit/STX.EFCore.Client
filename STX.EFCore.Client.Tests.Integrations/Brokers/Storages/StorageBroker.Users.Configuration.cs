// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STX.EFCore.Client.Tests.Integrations.Models.Users;

namespace STX.EFCore.Client.Tests.Integrations.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddUserConfigurations(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
        }
    }
}
