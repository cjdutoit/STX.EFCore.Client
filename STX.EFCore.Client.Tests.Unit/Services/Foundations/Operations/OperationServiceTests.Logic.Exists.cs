// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task ExistsAsyncShouldReturnTrueWhenEntityExists()
        {
            // Given
            User randomUser = CreateRandomUser();
            object[] inputIds = new object[] { randomUser.Id };
            List<User> storageUsers = new List<User> { randomUser };

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
                broker.FindEntityTypeAsync<User>())
                    .ReturnsAsync(entityTypeMock.Object);

            storageBrokerMock.Setup(broker =>
                broker.SelectAllAsync<User>())
                    .ReturnsAsync(storageUsers.AsQueryable());

            // When
            bool actualResult = await operationService.ExistsAsync<User>(inputIds);

            // Then
            actualResult.Should().BeTrue();

            storageBrokerMock.Verify(broker =>
                broker.FindEntityTypeAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExistsAsyncShouldReturnFalseWhenEntityDoesNotExist()
        {
            // Given
            object[] inputIds = new object[] { Guid.NewGuid() };
            List<User> storageUsers = new List<User>();

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
                broker.FindEntityTypeAsync<User>())
                    .ReturnsAsync(entityTypeMock.Object);

            storageBrokerMock.Setup(broker =>
                broker.SelectAllAsync<User>())
                    .ReturnsAsync(storageUsers.AsQueryable());

            // When
            bool actualResult = await operationService.ExistsAsync<User>(inputIds);

            // Then
            actualResult.Should().BeFalse();

            storageBrokerMock.Verify(broker =>
                broker.FindEntityTypeAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
