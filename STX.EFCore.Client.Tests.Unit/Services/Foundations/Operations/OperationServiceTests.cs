// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Moq;
using STX.EFCore.Client.Services.Foundations;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        private readonly Mock<DbContext> dbContextMock;
        private readonly OperationService operationService;

        public OperationServiceTests()
        {
            dbContextMock = new Mock<DbContext>();
            operationService = new OperationService(dbContextMock.Object);
        }

        private static User CreateRandomUser() =>
            CreateUserFiller().Create();

        private static Filler<User> CreateUserFiller()
        {
            var filler = new Filler<User>();
            filler.Setup();

            return filler;
        }
    }
}
