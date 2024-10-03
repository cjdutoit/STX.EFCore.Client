// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Extensions;

namespace STX.EFCore.Client.Brokers.Storages
{
    internal class StorageBroker : IStorageBroker
    {
        public DbContext DbContext { get; private set; }

        public StorageBroker(DbContext dbContext)
        {
            this.DbContext = dbContext;
            EntityFrameworkManager.ContextFactory = context => this.DbContext;
        }
    }
}
