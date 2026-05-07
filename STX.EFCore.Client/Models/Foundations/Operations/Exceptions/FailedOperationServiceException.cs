// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace STX.EFCore.Client.Models.Foundations.Operations.Exceptions
{
    internal class FailedOperationServiceException : Xeption
    {
        public FailedOperationServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
