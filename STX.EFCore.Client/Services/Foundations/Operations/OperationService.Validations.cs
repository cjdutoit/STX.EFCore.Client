// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using STX.EFCore.Client.Models.Foundations.Operations.Exceptions;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    internal partial class OperationService
    {
        private static void ValidateObjectIsNotNull<T>(T @object) where T : class
        {
            if (@object is null)
            {
                throw new NullOperationObjectException(
                    message: "Operation object is null, please fix and try again.");
            }
        }

        private static void ValidateCollectionIsNotNull<T>(IEnumerable<T> objects) where T : class
        {
            if (objects is null)
            {
                throw new NullOperationCollectionException(
                    message: "Operation collection is null, please fix and try again.");
            }
        }

        private static void ValidateObjectIdsIsNotNull(object[] objectIds)
        {
            if (objectIds is null)
            {
                throw new NullOperationObjectIdsException(
                    message: "Operation object ids is null, please fix and try again.");
            }
        }
    }
}
