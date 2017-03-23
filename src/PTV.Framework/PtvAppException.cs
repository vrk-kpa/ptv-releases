using System;
using System.Collections.Generic;
using PTV.Framework.Interfaces;

namespace PTV.Framework.ServiceManager
{
    public class PtvAppException : Exception
    {
        public string CodeIdentifier { get; protected set; }

        public IEnumerable<string> AdditionalParams { get; protected set; }

        public IVmBase AdditionalData { get; protected set; }

        public PtvAppException()
        {
        }

        public PtvAppException(IVmBase additionalData, string message, string codeIdentifier = null, IEnumerable<string> additionalParams = null) : this(message, null, codeIdentifier, additionalParams, additionalData)
        { }

        public PtvAppException(string message, string codeIdentifier = null, IEnumerable<string> additionalParams = null) : this(message, null, codeIdentifier, additionalParams)
        { }

        public PtvAppException(string message, Exception innerException, string codeIdentifier = null, IEnumerable<string> additionalParams = null, IVmBase additionalData = null) : base(message, innerException)
        {
            CodeIdentifier = codeIdentifier;
            AdditionalParams = additionalParams;
            AdditionalData = additionalData;
        }
    }

    public class LockException : PtvAppException
    {
        public LockException(string message, IEnumerable<string> additionalParams) : base(message,additionalParams:additionalParams)
        { }
        public LockException(string message, IEnumerable<string> additionalParams = null, IVmBase additionalData = null) : base(message, null, additionalParams:additionalParams, additionalData: additionalData)
        { }
    }

    public class ModifiedExistsException : PtvAppException
    {
        public ModifiedExistsException(string message, IEnumerable<string> additionalParams) : base(message, additionalParams: additionalParams)
        { }
    }

    public class PublishLanguageException : PtvAppException
    {
        public PublishLanguageException() : base()
        { }
    }

    public class PublishModifiedExistsException : PtvAppException
    {
        public PublishModifiedExistsException() : base()
        { }
    }

    public class OrganizationNotDeleteInUseException : PtvAppException
    {
        public OrganizationNotDeleteInUseException() : base()
        { }
    }

    public class RoleActionException : PtvAppException
    {
        public RoleActionException(string message, IEnumerable<string> additionalParams) : base(message, additionalParams: additionalParams)
        { }
    }

    public class DuplicityCheckException : PtvAppException
    {
        public DuplicityCheckException(string message) : base(message)
        { }
    }

    public class PtvServiceArgumentException : PtvAppException
    {
        public PtvServiceArgumentException(string message, IEnumerable<string> additionalParams) : base(message, additionalParams: additionalParams)
        { }
    }

    public class EntityNotFoundException : PtvAppException
    {
        public EntityNotFoundException(IVmBase additionalData, string message, string codeIdentifier, IEnumerable<string> additionalParams) : base(additionalData, message, codeIdentifier, additionalParams)
        { }

        public EntityNotFoundException(string message, string codeIdentifier, IEnumerable<string> additionalParams) : base(message, codeIdentifier, additionalParams)
        { }

        public EntityNotFoundException(string codeIdentifier, IEnumerable<string> additionalParams) : base(codeIdentifier, codeIdentifier, additionalParams)
        {}

        public EntityNotFoundException(IVmBase additionalData, string codeIdentifier, IEnumerable<string> additionalParams) : base(additionalData, codeIdentifier, codeIdentifier, additionalParams)
        { }
    }
}