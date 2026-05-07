// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using STX.EFCore.Client.Clients;
using STX.EFCore.Client.Services.Foundations.Operations;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Unit.Clients
{
    public partial class EFCoreClientTests
    {
        private readonly Mock<IOperationService> operationServiceMock;
        private readonly EFCoreClient efCoreClient;

        public EFCoreClientTests()
        {
            operationServiceMock = new Mock<IOperationService>();
            efCoreClient = new EFCoreClient(operationServiceMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
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
