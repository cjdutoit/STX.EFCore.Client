// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using STX.EFCore.Client.Models.Clients.Exceptions;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;
using Xeptions;

namespace STX.EFCore.Client.Tests.Unit.Clients
{
    public partial class EFCoreClientTests
    {
        [Fact]
        public async Task SelectAsyncShouldThrowClientValidationExceptionOnValidationErrorAsync()
        {
            // Given
            object[] someObjectIds = new object[] { Guid.NewGuid() };
            var someInnerXeption = new Xeption(message: GetRandomString());

            var operationValidationException =
                new OperationValidationException(
                    message: "Operation validation error occurred, fix the errors and try again.",
                    innerException: someInnerXeption);

            var expectedEFCoreClientValidationException =
                new EFCoreClientValidationException(
                    message: "EFCore client validation error occurred, fix the errors and try again.",
                    innerException: someInnerXeption);

            operationServiceMock.Setup(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationValidationException);

            // When
            ValueTask<User> selectTask = efCoreClient.SelectAsync<User>(someObjectIds);

            EFCoreClientValidationException actualException =
                await Assert.ThrowsAsync<EFCoreClientValidationException>(testCode: selectTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientValidationException);

            operationServiceMock.Verify(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldThrowClientValidationExceptionOnDependencyValidationErrorAsync()
        {
            // Given
            object[] someObjectIds = new object[] { Guid.NewGuid() };
            var someInnerXeption = new Xeption(message: GetRandomString());

            var operationDependencyValidationException =
                new OperationDependencyValidationException(
                    message: "Operation dependency validation error occurred, fix the errors and try again.",
                    innerException: someInnerXeption);

            var expectedEFCoreClientValidationException =
                new EFCoreClientValidationException(
                    message: "EFCore client validation error occurred, fix the errors and try again.",
                    innerException: someInnerXeption);

            operationServiceMock.Setup(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationDependencyValidationException);

            // When
            ValueTask<User> selectTask = efCoreClient.SelectAsync<User>(someObjectIds);

            EFCoreClientValidationException actualException =
                await Assert.ThrowsAsync<EFCoreClientValidationException>(testCode: selectTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientValidationException);

            operationServiceMock.Verify(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldThrowClientDependencyExceptionOnDependencyErrorAsync()
        {
            // Given
            object[] someObjectIds = new object[] { Guid.NewGuid() };
            var someInnerXeption = new Xeption(message: GetRandomString());

            var operationDependencyException =
                new OperationDependencyException(
                    message: "Operation dependency error occurred, contact support.",
                    innerException: someInnerXeption);

            var expectedEFCoreClientDependencyException =
                new EFCoreClientDependencyException(
                    message: "EFCore client dependency error occurred, contact support.",
                    innerException: someInnerXeption);

            operationServiceMock.Setup(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationDependencyException);

            // When
            ValueTask<User> selectTask = efCoreClient.SelectAsync<User>(someObjectIds);

            EFCoreClientDependencyException actualException =
                await Assert.ThrowsAsync<EFCoreClientDependencyException>(testCode: selectTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientDependencyException);

            operationServiceMock.Verify(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldThrowClientServiceExceptionOnServiceErrorAsync()
        {
            // Given
            object[] someObjectIds = new object[] { Guid.NewGuid() };
            var someInnerXeption = new Xeption(message: GetRandomString());

            var operationServiceException =
                new OperationServiceException(
                    message: "Operation service error occurred, contact support.",
                    innerException: someInnerXeption);

            var expectedEFCoreClientServiceException =
                new EFCoreClientServiceException(
                    message: "EFCore client service error occurred, contact support.",
                    innerException: someInnerXeption);

            operationServiceMock.Setup(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationServiceException);

            // When
            ValueTask<User> selectTask = efCoreClient.SelectAsync<User>(someObjectIds);

            EFCoreClientServiceException actualException =
                await Assert.ThrowsAsync<EFCoreClientServiceException>(testCode: selectTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientServiceException);

            operationServiceMock.Verify(service =>
                service.SelectAsync<User>(someObjectIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SelectAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            object[] someObjectIds = new object[] { Guid.NewGuid() };
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            operationServiceMock.Setup(service =>
                service.SelectAsync<User>(someObjectIds, cancelledToken))
                    .ThrowsAsync(new OperationCanceledException());

            // When
            ValueTask<User> selectTask = efCoreClient.SelectAsync<User>(someObjectIds, cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: selectTask.AsTask);

            operationServiceMock.Verify(service =>
                service.SelectAsync<User>(someObjectIds, cancelledToken),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
