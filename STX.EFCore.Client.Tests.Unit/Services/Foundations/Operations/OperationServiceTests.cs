// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Brokers.Storages;
using STX.EFCore.Client.Services.Foundations.Operations;
using STX.EFCore.Client.Tests.Unit.Brokers.Storages;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {

        private readonly TestDbContext dbContext;
        private readonly OperationService operationService;

        public OperationServiceTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            dbContext = new TestDbContext(options);
            IStorageBroker storageBroker = new StorageBroker(dbContext);
            OperationService operationService = new OperationService(storageBroker);
            this.operationService = new OperationService(storageBroker);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string CreateRandomString() =>
            new MnemonicString().GetValue();

        private static List<User> CreateRandomUsers() =>
            CreateUserFiller().Create(count: GetRandomNumber()).ToList();

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
