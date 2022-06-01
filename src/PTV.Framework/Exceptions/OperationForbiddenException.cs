using System;

namespace PTV.Framework.Exceptions
{
    public class OperationForbiddenException : Exception
    {
        public OperationForbiddenException()
        {

        }

        public OperationForbiddenException(string message)
            : base(message)
        {

        }

    }
}
