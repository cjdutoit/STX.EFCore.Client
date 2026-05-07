// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace STX.EFCore.Client.Models.Foundations.Operations.Exceptions
{
    internal class NullOperationObjectIdsException : Xeption
    {
        public NullOperationObjectIdsException(string message)
            : base(message) { }
    }
}
