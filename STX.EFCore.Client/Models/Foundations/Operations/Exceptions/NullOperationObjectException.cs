// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace STX.EFCore.Client.Models.Foundations.Operations.Exceptions
{
    internal class NullOperationObjectException : Xeption
    {
        public NullOperationObjectException(string message)
            : base(message) { }
    }
}
