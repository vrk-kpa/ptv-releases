/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class PtvDbTooManyConnectionsException : PtvAppException
    {
    }

    public class PtvArgumentException : PtvAppException
    {
        public string ParamName { get; set; }
        public PtvArgumentException(string message, string parameterName = "") : base(message)
        {
            ParamName = parameterName;
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
    public class LockNotAllowedException : PtvAppException
    {
        public LockNotAllowedException() : base()
        { }
    }

    public class PublishLanguageException : PtvAppException
    {
        public PublishLanguageException() : base()
        { }
    }

    public class ArchiveLanguageException : PtvAppException
    {
        public ArchiveLanguageException() : base()
        { }
    }

    public class WithdrawLanguageException : PtvAppException
    {
        public WithdrawLanguageException() : base()
        { }
    }

    public class RestoreLanguageException : PtvAppException
    {
        public RestoreLanguageException() : base()
        { }
    }

    public class PublishModifiedExistsException : PtvAppException
    {
        public PublishModifiedExistsException() : base()
        { }
    }

    public class WithdrawModifiedExistsException : PtvAppException
    {
        public WithdrawModifiedExistsException() : base()
        { }
    }

    public class WithdrawConnectedExistsException : PtvAppException
    {
        public WithdrawConnectedExistsException() : base()
        { }
    }

    public class RestoreModifiedExistsException : PtvAppException
    {
        public RestoreModifiedExistsException() : base()
        { }
    }

    public class OrganizationNotDeleteInUseException : PtvAppException
    {
        public OrganizationNotDeleteInUseException() : base()
        { }
    }

    public class OrganizationNotDeleteInUserUseException : PtvAppException
    {
        public OrganizationNotDeleteInUserUseException() : base()
        { }
    }

    public class OrganizationCyclicDependencyException : PtvAppException
    {
        public OrganizationCyclicDependencyException() : base()
        { }
    }

    public class OrganizationCannotPublishDeletedRootException : PtvAppException
    {
        public OrganizationCannotPublishDeletedRootException() : base()
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
    
    public class PtvValidationException : Exception, IError
    {
        public Dictionary<Guid, List<ValidationMessage>> ValidationMessages { get; protected set; }

        public PtvValidationException(Dictionary<Guid, List<ValidationMessage>> validationMessages, string message) : base(message)
        {
            ValidationMessages = validationMessages;
        }
    }
}
