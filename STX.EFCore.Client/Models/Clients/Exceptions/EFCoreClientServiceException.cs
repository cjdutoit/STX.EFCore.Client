// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace STX.EFCore.Client.Models.Clients.Exceptions
{
    public class EFCoreClientServiceException : Xeption
    {
        public EFCoreClientServiceException(string message, Xeption innerException)
            : base(message, innerException) { }
    }
}
