// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkReadAsyncShouldReturnAllTheRecords()
        {
            // Given
            List<User> randomUsers = CreateRandomUsers();
            List<User> inputUsers = randomUsers;
            List<User> storageUsers = inputUsers.ToList();
            List<User> expectedUsers = storageUsers.DeepClone();

            Mock<IEntityType> entityTypeMock = new Mock<IEntityType>();
            Mock<IKey> primaryKeyMock = new Mock<IKey>();
            Mock<IProperty> propertyMock = new Mock<IProperty>();

            PropertyInfo keyPropertyInfo = typeof(User).GetProperty("Id");
            propertyMock.Setup(p => p.PropertyInfo).Returns(keyPropertyInfo);
            propertyMock.Setup(p => p.Name).Returns(keyPropertyInfo.Name);
            propertyMock.Setup(p => p.ClrType).Returns(keyPropertyInfo.PropertyType);
            primaryKeyMock.Setup(pk => pk.Properties).Returns(new List<IProperty> { propertyMock.Object });
            entityTypeMock.Setup(et => et.FindPrimaryKey()).Returns(primaryKeyMock.Object);

            storageBrokerMock.Setup(broker =>
                broker.FindEntityType<User>())
                    .ReturnsAsync(entityTypeMock.Object);

            storageBrokerMock.Setup(broker =>
                broker.SelectAllAsync<User>())
                    .ReturnsAsync(storageUsers.AsQueryable());

            // When
            IEnumerable<User> actualUsers = await operationService.BulkReadAsync(objects: inputUsers);

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            storageBrokerMock.Verify(broker =>
                broker.FindEntityType<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
