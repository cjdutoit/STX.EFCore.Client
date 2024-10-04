// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EFxceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STX.EFCore.Client.Tests.Acceptance.Models.Users;

namespace STX.EFCore.Client.Tests.Acceptance.Brokers.Storages
{
    public partial class TestDbContext : EFxceptionsContext
    {
        private readonly IConfiguration configuration;

        public TestDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            string connectionString = this.configuration.GetConnectionString(name: "DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddUserConfigurations(modelBuilder.Entity<User>());
        }
    }
}
