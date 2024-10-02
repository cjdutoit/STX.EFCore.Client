// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Clients;
using STX.EFCore.Client.Tests.Acceptance.Brokers.Storages;
using STX.EFCore.Client.Tests.Acceptance.Models.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Acceptance.Clients
{
    public partial class OperationServiceTests
    {
        private readonly IEFCoreClient efCoreClient;

        public OperationServiceTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAcceptanceDb").Options;

            TestDbContext dbContext = new TestDbContext(options);
            this.efCoreClient = new EFCoreClient(dbContext);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<User> CreateRandomUsers(int count) =>
            CreateUserFiller().Create(count).ToList();

        private static User CreateRandomUser() =>
            CreateUserFiller().Create();

        private static Filler<User> CreateUserFiller()
        {
            var filler = new Filler<User>();
            filler.Setup().OnProperty(user => user.Id).Use(() => Guid.NewGuid());

            return filler;
        }
    }
}
