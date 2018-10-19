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

namespace PTV.Framework
{
    /// <summary>
    /// System messages for inner app events
    /// </summary>
    public class CoreMessages
    {
        public const string RollbackFailed = "Rollback operation on DB context has failed";
        public const string CommitFailed = "commit operation on DB context has failed";
        public const string DbContextInUseAlready = "DB context is already performing operation, nested transactions are not allowed";
        public const string IncorrectTranslationDefinition = "Translation of properties is wrong, incompatible types. Check and fix translation definition";
        public const string AnonymousSaveNotAllowed = "Anonymous changes to data are not allowed!";
        public const string EntityNotFoundToUpdate = "Cannot find entity to update!";
        public const string EntityReplacedInUpdate = "Existing entity is replaced by another instance during update, wrong order of definitions???";
        public const string EntityExistsInCreate = "New entity should be created in create process, but instance is already assigned, wrong order of definitions???";
        public const string WrongRequestFormat = "Request does not contain valid data in JSON format";
        public const string TranslatorWrongVersioningUsage = "Call UseDataContextCreate or UseDataContextUpdate before UseVersioning definition!";
        public const string EntityAccessDenied = "Access denied to entity!";
        public const string TooManyRequests = "Too many requests sent to server, try again later";

        /// <summary>
        /// OpenApi related messages
        /// </summary>
        public class OpenApi
        {
            public const string RecordNotFound = "Record not found or user does not have rights for it!";
            public const string GuidMalFormatted = "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).";
            public const string RequestMalFormattedShort = "The field \'{0}\' is invalid.";
            public const string RequestMalFormatted = "The field is invalid. Please use one of these: \'{0}\'.";
            public const string RequestIsNull = "The request must contain data.";
            public const string NotEnum = "The type is not enumeration.";
            public const string NotValidEnum = "The field \'{0}\' has not valid enumeration value.";
            public const string MustBeNumeric = "The value must be numeric.";
            public const string UnknownProperty = "Unknown property: {0}";
            public const string NullNotAllowedInProperty = "Null value not allowed in the item property name: {0}.";
            public const string NullNotAllowedInList = "Null value not allowed in the list.";
            public const string CodeNotFound = "The code \'{0}\' was not found!";
            public const string RequiredValueNotFound = "{0} - Required value \'{1}\' was not found!";
            public const string RequiredLanguageValueNotFound = "Required value was not found with language code \'{0}\'!";
            public const string RequiredValueWithLanguageAndTypeNotFound = "Required value with type \'{0}\' was not found with language code \'{1}\'!";
            public const string DateMalFormatted = "The date parameter is invalid.";
            public const string RequiredIf = "The field is required when \'{0}\' has value \'{1}\'.";
            public const string EntityNotFound = "{0} with id \'{1}\' not found!";
            public const string RelationshipAlreadyExists = "Relationship between service \'{0}\' and channel \'{1}\' already exists!";
            public const string ServiceChannelsAdded = "{0} service channels added for service \'{1}\'";
            public const string ServiceServiceChannelAdded = "Relationship data for service channel(s) \'{0}\' and service \'{1}\' added or updated.";
            public const string ExternalSourceExists = "The external source \'{0}\' already exists. Please use update endpoint!";
            public const string ExternalSourceNotExists = "The external source \'{0}\' does not exists. Please use POST endpoint or other sourceId!";
            public const string ExternalSourceExistsUpdate = "The external source \'{0}\' already exists for entity \'{1}\'. Please use another sourceId!";
            public const string RelationIdNotFound = "The relation id could not be found. Check scopes for authentication token!";
            public const string ExternalSourceForOtherExists = "The external source \'{0}\' already exists for {1} entity. Please use other sourceId!";
            public const string ExternalSourceMalFormatted = "The external source cannot be set. Check values!";
            public const string TypeCannotBeChanged = "The type cannot be changed. Please set the item as \'Deleted\' and create a new one.";
            public const string ListRequiredIfProperty = "The item with \'{0} = {1}\' is required when \'{2}\' has value \'{3}\'.";
            public const string InternalServerErrorDescripton = "An error has occured on the server.";
            public const string OidExists = "Organization with same Oid '{0}' already exists!";
            public const string LocalizedDuplicitiesAreNotAllowed = "Localized duplicates are not allowed!";
            public const string LocalizedDuplicitiesOfPropertyAreNotAllowed = "Localized duplicates of \'{0}\' are not allowed!";
            public const string DuplicitiesAreNotAllowed = "Duplicates of \'{0}\' are not allowed!";
            public const string EnumValueIsNotValid = "\'{0}\' is not valid value of \'{1}\'!";
            public const string EnumValueIsNotAllowed = "\'{0}\' is not allowed value of \'{1}\'! Please use one of these: \'{2}\'.";
            public const string BadRequestGeneralMessage = "The request object is missing or contains invalid values.";
            public const string NotFoundGeneralMessage = "Entity not found with given identifier.";
            public const string UnauthorizedGeneralMessage = "Authentication required (bearer token missing from header or not valid).";
            public const string ForbiddenGeneralMessage = "Requested operation is not permitted.";
            public const string ValueNotEmpty = "At least five characters are required for this field.";
            public const string PublishingStatusNotValid = "You cannot set Publishing status as {0} when current one is {1}.";
            public const string SuitableVersionNotFound = "Suitable version of {0} with id \'{1}\' not found!";
            public const string ValidStreetAddress = "StreetNumber is required if StreetAddress is provided.";
            public const string ValidStreetAddressWithCoordinates = "StreetNumber or coordinates (Latitude, Longitude) are required if StreetAddress is provided.";
            public const string ChannelNotVisibleForAll = "Service channel '{0}' is restricted only for defined organization. You do not have permission to update data!";
            public const string UserOrganizationRequired = "Organization \'{0}\' is not a valid user organization. You do not have permission to update data!";
            public const string ListRequiredToMatchDisplayNameTypesMissingNames = "All language/type pairs defined in {0} must be specified. Missing: {1}/{2}";
            public const string ListRequiredToMatchDisplayNameTypesMissingTypes = "{0} is required for each language in collection. Missing: {1}";
            public const string AdditionalInfoForConnection = "ServiceHours and ContactDetails are only allowed for Service location channel! Service channel '{0}' is of type '{1}'.";
            public const string TargetGroupRequired = "Target group '{0}' or one of the sub target groups is required if {1} are attached.";
            public const string MustBeServiceLocationChannel = "Service channel must be of type Service location channel! Service channel '{0}' is of type '{1}'.";
            public const string AllMustBeServiceLocationChannels = "{0} are only allowed for Service location channel! Following service channels are not service location channels '{1}'.";
            public const string OrganizationExceptionMaxHierarchyLevel = "(CoreMessage) You already have max levels or organizations. You cannot create more that {0}-levels!";
            public const string TooManyItems = "There were too many items within result set. Only 100 items are allowed. Use more restrictive search criteria!";
            public const string DublicateItemsNotAllowed = "Dublicate items not allowed! Please check your request related to these items: {0}.";
            public const string SameNameAndSummaryNotAllowed = "Name and summary are not allowed to be the same. Please update either of them for language code '{0}'.";
        }

        public const string UseAddCollectionInstead = "Use AddCollection method for IEnumerable types. Translator '{0} <-> {1}', between property types {2} and {3}'";
        public const string IncorrectUsageTranslation = "Incorrect input types for AddNavigationOneMany, must be from Instance to Collection. Translator '{0} <-> {1}', between property types {2} and {3}'";
        public const string DbSetNotFound = "Requested DbSet '{0}' not found!";
        public const string EntityHasNoPrimaryKey = "Entity {0} has not primary key defined!";
        public const string OutputEntityContainsNulls = "Entity {0} contains null  value(s) in accessed navigation property! Translator '{1} - {2}'";
        public const string MainPublishedEntityLocked = "Cannot update previous (Published) version if newer version (Modified) exists!";
        public const string MainPublishedEntityLockedId = "Service.VersionException.MainPublishedEntityLocked";
        public const string CannotProcessThisVersion = "Cannot update entity which has non-allowed publishing status!";
    }
}
