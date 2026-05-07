// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace STX.EFCore.Client.Models.Foundations.Operations.Exceptions
{
    internal class OperationValidationException : Xeption
    {
        public OperationValidationException(string message, Xeption innerException)
            : base(message, innerException) { }
    }
}
