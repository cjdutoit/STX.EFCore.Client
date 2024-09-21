// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
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
