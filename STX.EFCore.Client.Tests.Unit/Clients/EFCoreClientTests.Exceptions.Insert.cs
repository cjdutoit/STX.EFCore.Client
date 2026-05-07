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
        public async Task InsertAsyncShouldThrowClientValidationExceptionOnValidationErrorAsync()
        {
            // Given
            User someUser = CreateRandomUser();
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
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationValidationException);

            // When
            ValueTask<User> insertTask = efCoreClient.InsertAsync(someUser);

            EFCoreClientValidationException actualException =
                await Assert.ThrowsAsync<EFCoreClientValidationException>(testCode: insertTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientValidationException);

            operationServiceMock.Verify(service =>
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InsertAsyncShouldThrowClientValidationExceptionOnDependencyValidationErrorAsync()
        {
            // Given
            User someUser = CreateRandomUser();
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
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationDependencyValidationException);

            // When
            ValueTask<User> insertTask = efCoreClient.InsertAsync(someUser);

            EFCoreClientValidationException actualException =
                await Assert.ThrowsAsync<EFCoreClientValidationException>(testCode: insertTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientValidationException);

            operationServiceMock.Verify(service =>
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InsertAsyncShouldThrowClientDependencyExceptionOnDependencyErrorAsync()
        {
            // Given
            User someUser = CreateRandomUser();
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
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationDependencyException);

            // When
            ValueTask<User> insertTask = efCoreClient.InsertAsync(someUser);

            EFCoreClientDependencyException actualException =
                await Assert.ThrowsAsync<EFCoreClientDependencyException>(testCode: insertTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientDependencyException);

            operationServiceMock.Verify(service =>
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InsertAsyncShouldThrowClientServiceExceptionOnServiceErrorAsync()
        {
            // Given
            User someUser = CreateRandomUser();
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
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationServiceException);

            // When
            ValueTask<User> insertTask = efCoreClient.InsertAsync(someUser);

            EFCoreClientServiceException actualException =
                await Assert.ThrowsAsync<EFCoreClientServiceException>(testCode: insertTask.AsTask);

            // Then
            actualException.Should().BeEquivalentTo(expectedEFCoreClientServiceException);

            operationServiceMock.Verify(service =>
                service.InsertAsync(someUser, It.IsAny<CancellationToken>()),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task InsertAsyncShouldNotWrapOperationCanceledExceptionAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            operationServiceMock.Setup(service =>
                service.InsertAsync(someUser, cancelledToken))
                    .ThrowsAsync(new OperationCanceledException());

            // When
            ValueTask<User> insertTask = efCoreClient.InsertAsync(someUser, cancelledToken);

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(testCode: insertTask.AsTask);

            operationServiceMock.Verify(service =>
                service.InsertAsync(someUser, cancelledToken),
                    Times.Once);

            operationServiceMock.VerifyNoOtherCalls();
        }
    }
}
