// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace STX.EFCore.Client.Brokers.Storages
{
    internal interface IStorageBroker
    {
        DbContext DbContext { get; }
    }
}
