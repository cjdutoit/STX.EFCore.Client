// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace STX.EFCore.Client.Models.Clients.Exceptions
{
    public class EFCoreClientValidationException : Xeption
    {
        public EFCoreClientValidationException(string message, Xeption innerException)
            : base(message, innerException) { }
    }
}
