// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using STX.EFCore.Client.Tests.Integrations.Brokers.Storages;
using STX.EFCore.Client.Tests.Integrations.Models.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Integrations.Tests
{
    public partial class StorageBrokerTests
    {
        private readonly IStorageBroker storageBroker;

        public StorageBrokerTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();
            this.storageBroker = new StorageBroker(configuration);
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
