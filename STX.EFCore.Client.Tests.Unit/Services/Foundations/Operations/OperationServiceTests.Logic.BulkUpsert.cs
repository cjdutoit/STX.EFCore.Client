// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkUpsertAsyncShouldInsertNewAndUpdateExistingWithoutTransaction()
        {
            // Given
            bool useTransaction = false;
            List<User> existingUsers = CreateRandomUsers();
            List<User> newUsers = CreateRandomUsers();
            List<User> inputUsers = existingUsers.Concat(newUsers).ToList();

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
                    .ReturnsAsync(existingUsers.AsQueryable());

            // When
            await operationService.BulkUpsertAsync(objects: inputUsers, useTransaction);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.FindEntityTypeAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(
                    It.Is<List<User>>(list => list.Count == newUsers.Count),
                    default),
                        Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkUpdateAsync(
                    It.Is<List<User>>(list => list.Count == existingUsers.Count),
                    default),
                        Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(default),
                    Times.Once);

            foreach (var user in inputUsers)
            {
                storageBrokerMock.Verify(broker =>
                    broker.UpdateObjectStateAsync(user, EntityState.Detached),
                        Times.Once);
            }

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpsertAsyncShouldInsertNewAndUpdateExistingWithTransaction()
        {
            // Given
            bool useTransaction = true;
            List<User> existingUsers = CreateRandomUsers();
            List<User> newUsers = CreateRandomUsers();
            List<User> inputUsers = existingUsers.Concat(newUsers).ToList();

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
                    .ReturnsAsync(existingUsers.AsQueryable());

            storageBrokerMock.Setup(broker =>
                broker.BeginTransactionAsync(default))
                    .ReturnsAsync(dbContextTransactionMock.Object);

            // When
            await operationService.BulkUpsertAsync(objects: inputUsers, useTransaction);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.FindEntityTypeAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BeginTransactionAsync(default),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(
                    It.Is<List<User>>(list => list.Count == newUsers.Count),
                    default),
                        Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkUpdateAsync(
                    It.Is<List<User>>(list => list.Count == existingUsers.Count),
                    default),
                        Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(default),
                    Times.Once);

            dbContextTransactionMock.Verify(transaction =>
                transaction.CommitAsync(default),
                    Times.Once);

            foreach (var user in inputUsers)
            {
                storageBrokerMock.Verify(broker =>
                    broker.UpdateObjectStateAsync(user, EntityState.Detached),
                        Times.Once);
            }

            dbContextTransactionMock.Verify(transaction =>
                transaction.Dispose(),
                    Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
            dbContextTransactionMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpsertAsyncShouldOnlyInsertWhenAllRecordsAreNew()
        {
            // Given
            bool useTransaction = false;
            List<User> newUsers = CreateRandomUsers();
            List<User> inputUsers = newUsers;

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
                    .ReturnsAsync(new List<User>().AsQueryable());

            // When
            await operationService.BulkUpsertAsync(objects: inputUsers, useTransaction);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.FindEntityTypeAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(
                    It.Is<List<User>>(list => list.Count == newUsers.Count),
                    default),
                        Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkUpdateAsync(It.IsAny<List<User>>(), default),
                    Times.Never);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(default),
                    Times.Once);

            foreach (var user in inputUsers)
            {
                storageBrokerMock.Verify(broker =>
                    broker.UpdateObjectStateAsync(user, EntityState.Detached),
                        Times.Once);
            }

            storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task BulkUpsertAsyncShouldOnlyUpdateWhenAllRecordsExist()
        {
            // Given
            bool useTransaction = false;
            List<User> existingUsers = CreateRandomUsers();
            List<User> inputUsers = existingUsers;

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
                    .ReturnsAsync(existingUsers.AsQueryable());

            // When
            await operationService.BulkUpsertAsync(objects: inputUsers, useTransaction);

            // Then
            storageBrokerMock.Verify(broker =>
                broker.FindEntityTypeAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SelectAllAsync<User>(),
                    Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.BulkInsertAsync(It.IsAny<List<User>>(), default),
                    Times.Never);

            storageBrokerMock.Verify(broker =>
                broker.BulkUpdateAsync(
                    It.Is<List<User>>(list => list.Count == existingUsers.Count),
                    default),
                        Times.Once);

            storageBrokerMock.Verify(broker =>
                broker.SaveChangesAsync(default),
                    Times.Once);

            foreach (var user in inputUsers)
            {
                storageBrokerMock.Verify(broker =>
                    broker.UpdateObjectStateAsync(user, EntityState.Detached),
                        Times.Once);
            }

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
