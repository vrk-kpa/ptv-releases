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
import { defineMessages } from 'util/react-intl'

const messages = defineMessages({
  // save, publish, withdraw, restore, archive
  messageEntitySaved: {
    id: 'Entity.MessageSaved',
    defaultMessage: 'Tiedot tallennettu.'
  },
  messageAddServiceAndChannel: {
    id: 'ServiceAndChannel.AddServiceAndChannel.MessageSaved',
    defaultMessage: 'Liitokset tallennettu.'
  },
  messageEntityPublished: {
    id: 'Entity.MessagePublished',
    defaultMessage: 'Tiedot julkaistu.'
  },
  messageEntitySchedulePublish: {
    id: 'Entity.MessageSchedulePublish',
    defaultMessage: 'Publish date saved.'
  },
  messageEntityScheduleArchive: {
    id: 'Entity.MessageScheduleArchive',
    defaultMessage: 'Archive date saved.'
  },
  messageEntityWithdrawn: {
    id: 'Entity.MessageWithdrawn',
    defaultMessage: 'Tiedot palautettu luonnokseksi.'
  },
  messageEntityRestored: {
    id: 'Entity.MessageRestored',
    defaultMessage: 'Tietot palautettu arkistosta.'
  },
  messageEntityArchived: {
    id: 'Entity.MessageArchived',
    defaultMessage: 'Tiedot arkistoitu.'
  },
  messageEntityCannotBeEdited: {
    id: 'Entity.MessageCannotBeEdited',
    defaultMessage: 'Muokataksesi tietoja, ne tulee ensin palauttaa arkistosta.'
  },
  messageEntityCannotBePublished: {
    id: 'Entity.MessageCannotBePublished',
    defaultMessage: 'Julkaistaksesi tiedot, ne tulee ensin palauttaa arkistosta.'
  },
  messageServiceSaved: {
    id: 'Service.AddService.MessageSaved',
    defaultMessage: 'Onnittelut! Palvelun tallennus onnistui.'
  },
  messageOrganizationSaved: {
    id: 'Organization.AddOrganization.MessageSaved',
    defaultMessage: 'Onnittelut! Organisaatio tallennus onnistui.'
  },
  messageServicePublished: {
    id: 'Service.EditService.MessagePublished',
    defaultMessage: 'Onnittelut! Palvelun julkaisu onnistui.'
  },
  messageServicePublishedException: {
    id: 'Service.Publish.MessageFailed',
    defaultMessage: 'Toteutustapa ja tuottaja: Valitse vähintään yksi tuottajatieto.'
  },
  messageServicePublishedStep2Exception: {
    id: 'Service.Publish.Step2.MessageFailed',
    defaultMessage: 'Classification and keywords: Select all mandatory fields.'
  },
  messageServiceSaveOrganizationMissingException: {
    id: 'Service.Save.OrganizationMissing.MessageFailed',
    defaultMessage: 'Organization connected to service is missing.'
  },
  messageChannelSaveOrganizationMissingException: {
    id: 'Channel.Save.OrganizationMissing.MessageFailed',
    defaultMessage: 'Organization connected to channel is missing.'
  },
  messageServiceWithdrawed: {
    id: 'Service.EditService.MessageWithdrawed',
    defaultMessage: 'Onnittelut! Service withdrawed.'
  },
  messageServiceRestored: {
    id: 'Service.EditService.MessageRestored',
    defaultMessage: 'Onnittelut! Service restored.'
  },
  messageOrganizationPublished: {
    id: 'Organization.EditOrganization.MessagePublished',
    defaultMessage: 'Onnittelut! Organisaatio julkaisu onnistui.'
  },
  messageOrganizationWithdrawed: {
    id: 'Organization.EditOrganization.MessageWithdrawed',
    defaultMessage: 'Onnittelut! Organisaatio withdrawed.'
  },
  messageOrganizationRestored: {
    id: 'Organization.EditOrganization.MessageRestored',
    defaultMessage: 'Onnittelut! Organisaatio restored.'
  },
  messageServiceDeleted: {
    id: 'Service.EditService.MessageDeleted',
    defaultMessage: 'Onnittelut! Palvelu on poistettu.'
  },
  messageOrganizationDeleted: {
    id: 'Organization.EditOrganization.MessageDeleted',
    defaultMessage: 'Onnittelut! Organisaatio on poistettu.'
  },
  messageElectronicChannelSaved: {
    id: 'Service.AddElectronicChannel.MessageSaved',
    defaultMessage: 'Onnittelut! Asiointikanavan tallennus onnistui.'
  },
  messageChannelPublished: {
    id: 'Channel.Published.Successfully',
    defaultMessage: 'Onnittelut! Asiointikanavan julkaisu onnistui.'
  },
  messageChannelWithdrawed: {
    id: 'Channel.Withdraw.Successfully',
    defaultMessage: 'Onnittelut! Channel withdrawed.'
  },
  messageChannelRestored: {
    id: 'Channel.Restore.Successfully',
    defaultMessage: 'Onnittelut! Channel restored.'
  },
  messageChannelDeleted: {
    id: 'Channel.Deleted.Successfully',
    defaultMessage: 'Onnittelut! Asiointikanava on poistettu'
  },
  messagePhoneChannelSaved: {
    id: 'Service.AddPhoneChannel.MessageSaved',
    defaultMessage: 'Onnittelut! Asiointikanavan tallennus onnistui.'
  },
  messageLocationChannelSaved: {
    id: 'Service.AddLocationChannel.MessageSaved',
    defaultMessage: 'Onnittelut! Asiointikanavan tallennus onnistui.'
  },
  messageWebPageChannelSaved: {
    id: 'Service.AddWebPageChannel.MessageSaved',
    defaultMessage: 'Onnittelut! Asiointikanavan tallennus onnistui.'
  },
  messageServiceStepSaved: {
    id: 'Service.EditService.MessageStepSaved',
    defaultMessage: 'Onnittelut! Tiedot tallennettu.'
  },
  messageOrganizationStepSaved: {
    id: 'Organization.OrganizationStep.MessageSaved',
    defaultMessage: 'Onnittelut! Tiedot tallennettu.'
  },
  messageChannelStepSaved: {
    id: 'Service.ChannelStep.MessageSaved',
    defaultMessage: 'Onnittelut! Tiedot tallennettu.'
  },

  messageInvalidArgument: {
    id: 'Service.Exception.InvalidArgument',
    defaultMessage: 'Invalid arguments : {0}'
  },
  messageDuplicityArgument: {
    id: 'Organization.Exception.DuplicityCheck',
    defaultMessage: 'OID monistaa : {0}!'
  },
  messageOidFormatArgument: {
    id: 'Organization.Exception.OidFormat',
    defaultMessage: 'OID {0} is in ivalid format!'
  },
  messageSHidFormatArgument: {
    id: 'Channel.Exception.OidFormat',
    defaultMessage: 'Social and health service location ID {0} is in ivalid format!'
  },
  messageServiceNotFound: {
    id: 'Service.Exception.NotFound',
    defaultMessage: 'Service with ID {0} not found!'
  },
  messageChannelNotFound: {
    id: 'ServiceChannel.Exception.NotFound',
    defaultMessage: 'Channel with ID {0} not found!'
  },
  messageOrganizationNotFound: {
    id: 'Organization.Exception.NotFound',
    defaultMessage: 'Organization with ID {0} not found!'
  },
  messagePublishServiceAndChannel: {
    id: 'ServiceAndChannel.PublishServiceAndChannel.MessageSaved',
    defaultMessage: 'Onnittelut!'
  },
  messageServiceLocked: {
    id: 'Service.Exception.MessageLock',
    defaultMessage: 'Service is locked by {0}!'
  },
  messageServiceCollectionLocked: {
    id: 'ServiceCollection.Exception.MessageLock',
    defaultMessage: 'Service collection is locked by {0}!'
  },
  messageChannelLocked: {
    id: 'Channel.Exception.MessageLock',
    defaultMessage: 'Channel is locked by {0}!'
  },
  messageNewVersionExists: {
    id: 'Common.EntityException.UnableEdit',
    defaultMessage: 'Modified version already exist, please use that one for editing.'
  },
  messageNotVisibleLanguage: {
    id: 'Common.EntityException.NotVisibleLanguage',
    defaultMessage: 'At least one language version must be chosen as visible.'
  },
  messagePublishModifiedExists: {
    id: 'Common.EntityException.PublishModifiedExists',
    defaultMessage: 'Unable to restore data to Modified state because another Modified version already exists.'
  },
  MessageWithdrawModifiedExists: {
    id: 'Common.EntityException.WithdrawModifiedExists',
    defaultMessage: 'Unable to withdraw data because another Modified version already exists.'
  },
  MessageRestoreModifiedExists: {
    id: 'Common.EntityException.RestoreModifiedExists',
    defaultMessage: 'Unable to restore data because another Modified version already exists.'
  },
  messageOrganizationLocked: {
    id: 'Organization.Exception.MessageLock',
    defaultMessage: 'Organization is locked by {0}!'
  },
  messageOrganizationCannotRemove:{
    id:'Organization.Exception.CannotRemove',
    defaultMessage: 'Organization cannot be removed, because it is used in services or channels!'
  },
  messageOrganizationUserInUseCannotRemove:{
    id:'Organization.Exception.UserInUse.CannotRemove',
    defaultMessage: 'Organization cannot be removed, because it is used as main organization for user!'
  },
  messageOrganizationCyclicDependency:{
    id:'Organization.Exception.CyclicDependency',
    defaultMessage: 'Assigned parent organization will cause cyclic dependency!'
  },
  messageOrganizationWithdrawConnection: {
    id: 'Organization.Exception.WithdrawConnection',
    defaultMessage: 'Organization can not be withdrawed! Organization is already used.'
  },
    // Service Role Exceptions messages
  messageServiceRoleActionAdd: {
    id: 'Service.RoleException.MessageAdd',
    defaultMessage: 'Service can not be added! User {0} with role {1} has no right to add service.'
  },
  messageServiceRoleActionSave: {
    id: 'Service.RoleException.MessageSave',
    defaultMessage: 'Service can not be saved! User {0} with role {1} has no right to save service.'
  },
  messageServiceRoleActionPublish: {
    id: 'Service.RoleException.MessagePublish',
    defaultMessage: 'Service can not be published! User {0} with role {1} has no right to publish service.'
  },
  messageServiceRoleActionWithdraw: {
    id: 'Service.RoleException.MessageWithdraw',
    defaultMessage: 'Service can not be withdrawed! User {0} with role {1} has no right to withdraw service.'
  },
  messageServiceRoleActionRestore: {
    id: 'Service.RoleException.MessageRestore',
    defaultMessage: 'Service can not be restored! User {0} with role {1} has no right to restore service.'
  },
  messageServiceRoleActionDelete: {
    id: 'Service.RoleException.MessageDelete',
    defaultMessage: 'Service can not be deleted! User {0} with role {1} has no right to delete service.'
  },
  messageServiceRoleActionLock: {
    id: 'Service.RoleException.MessageLock',
    defaultMessage: 'Service can not be locked! User {0} with role {1} has no right to lock channel.'
  },
   // Service collection Role Exceptions messages
   messageServiceCollectionRoleActionAdd: {
    id: 'ServiceCollection.RoleException.MessageAdd',
    defaultMessage: 'Service collection can not be added! User {0} with role {1} has no right to add service collection.'
  },
  messageServiceCollectionRoleActionSave: {
    id: 'ServiceCollection.RoleException.MessageSave',
    defaultMessage: 'Service collection can not be saved! User {0} with role {1} has no right to save service collection.'
  },
  messageServiceCollectionRoleActionPublish: {
    id: 'ServiceCollection.RoleException.MessagePublish',
    defaultMessage: 'Service collection can not be published! User {0} with role {1} has no right to publish service collection.'
  },
  messageServiceCollectionRoleActionWithdraw: {
    id: 'ServiceCollection.RoleException.MessageWithdraw',
    defaultMessage: 'Service collection can not be withdrawed! User {0} with role {1} has no right to withdraw service collection.'
  },
  messageServiceCollectionRoleActionRestore: {
    id: 'ServiceCollection.RoleException.MessageRestore',
    defaultMessage: 'Service collection can not be restored! User {0} with role {1} has no right to restore service collection.'
  },
  messageServiceCollectionRoleActionDelete: {
    id: 'ServiceCollection.RoleException.MessageDelete',
    defaultMessage: 'Service collection can not be deleted! User {0} with role {1} has no right to delete service collection.'
  },
  messageServiceCollectionRoleActionLock: {
    id: 'ServiceCollection.RoleException.MessageLock',
    defaultMessage: 'Service collection can not be locked! User {0} with role {1} has no right to lock service collection.'
  },
    // Channel Role Exceptions messages
  messageChannelRoleActionAdd: {
    id: 'Channel.RoleException.MessageAdd',
    defaultMessage: 'Channel can not be added! User {0} with role {1} has no right to add channel.'
  },
  messageChannelRoleActionSave: {
    id: 'Channel.RoleException.MessageSave',
    defaultMessage: 'Channel can not be saved! User {0} with role {1} has no right to save channel.'
  },
  messageChannelRoleActionPublish: {
    id: 'Channel.RoleException.MessagePublish',
    defaultMessage: 'Channel can not be published! User {0} with role {1} has no right to publish channel.'
  },
  messageChannelRoleActionWithdraw: {
    id: 'Channel.RoleException.MessageWithdraw',
    defaultMessage: 'Channel can not be withdrawed! User {0} with role {1} has no right to withdraw channel.'
  },
  messageChannelRoleActionRestore: {
    id: 'Channel.RoleException.MessageRestore',
    defaultMessage: 'Channel can not be restored! User {0} with role {1} has no right to restore channel.'
  },
  messageChannelRoleActionDelete: {
    id: 'Channel.RoleException.MessageDelete',
    defaultMessage: 'Channel can not be deleted! User {0} with role {1} has no right to delete channel.'
  },
  messageChannelRoleActionLock: {
    id: 'Channel.RoleException.MessageLock',
    defaultMessage: 'Channel can not be locked! User {0} with role {1} has no right to lock channel.'
  },
    // Organization Role Exceptions messages
  messageOrganizationRoleActionAdd: {
    id: 'Organization.RoleException.MessageAdd',
    defaultMessage: 'Organization can not be added! User {0} with role {1} has no right to add organization.'
  },
  messageOrganizationRoleActionSave: {
    id: 'Organization.RoleException.MessageSave',
    defaultMessage: 'Organization can not be saved! User {0} with role {1} has no right to save organization.'
  },
  messageOrganizationRoleActionPublish: {
    id: 'Organization.RoleException.MessagePublish',
    defaultMessage: 'Organization can not be published! User {0} with role {1} has no right to publish organization.'
  },
  messageOrganizationRoleActionWithdraw: {
    id: 'Organization.RoleException.MessageWithdraw',
    defaultMessage: 'Organization can not be withdrawed! User {0} with role {1} has no right to withdraw organization.'
  },
  messageOrganizationRoleActionRestore: {
    id: 'Organization.RoleException.MessageRestore',
    defaultMessage: 'Organization can not be restored! User {0} with role {1} has no right to restore organization.'
  },
  messageOrganizationRoleActionDelete: {
    id: 'Organization.RoleException.MessageDelete',
    defaultMessage: 'Organization can not be deleted! User {0} with role {1} has no right to delete organization.'
  },
  messageOrganizationRoleActionLock: {
    id: 'Organization.RoleException.MessageLock',
    defaultMessage: 'Organization can not be locked! User {0} with role {1} has no right to lock organization.'
  },
  // Connection Role Exceptions messages
  messageConnectionRoleActionSave: {
    id: 'ServiceAndChannel.RoleException.MessageSave',
    defaultMessage: 'Connection can not be saved! User {0} with role {1} has no right to save connection.'
  },
  messageConnectionRoleActionPublish: {
    id: 'ServiceAndChannel.RoleException.MessagePublish',
    defaultMessage: 'Connection can not be published! User {0} with role {1} has no right to publish all items.'
  },
  // Annotation messages
  messageAnnotationSucceeded: {
    id: 'Service.Annotations.MessageSucceeded',
    defaultMessage: 'Annotations read successfully.'
  },
  messageAnnotationFailed: {
    id: 'Service.AnnotationException.MessageFailed',
    defaultMessage: 'Annotation tool failed.'
  },
  messageAnnotationEmpty: {
    id: 'Service.AnnotationException.MessageEmpty',
    defaultMessage: 'One of those fields must be filled: Name, ShortDescription, Description, Classification, Target groups, Service class, Life event or Standard industrial class.'
  },
  messageOrganizationTypeIsNotAllowed: {
    id: 'Organization.Exception.OrganizationType',
    defaultMessage: 'Organization type {0} is not allowed!'
  },
  // Service organizer message
  messageOrganizerDeleteException: {
    id: 'Service.OrganizerUpdateException.MessageFailed',
    defaultMessage: 'Can not remove all organizatios used as self producers!'
  },
  // Version manager message
  messageVersionManagerMainEntityException: {
    id: 'Service.VersionException.MainPublishedEntityLocked',
    defaultMessage: 'Cannot update previous (Published) version if newer version (Modified) exists!'
  },
  messageServiceStep1Updated: {
    id: 'Service.EditService.MessageStep1Saved',
    defaultMessage: 'Onnittelut! Tiedot tallennettu. You have made changes that can affect the producer info, please check the producer info.'
  },
  // General description role messages
  messageGeneralDescriptionRoleActionAdd: {
    id: 'GeneralDescription.RoleException.MessageAdd',
    defaultMessage: 'General description can not be added! User {0} with role {1} has no right to add general description.'
  },
  messageGeneralDescriptionRoleActionSave: {
    id: 'GeneralDescription.RoleException.MessageSave',
    defaultMessage: 'General description can not be saved! User {0} with role {1} has no right to save general description.'
  },
  messageGeneralDescriptionRoleActionPublish: {
    id: 'GeneralDescription.RoleException.MessagePublish',
    defaultMessage: 'General description can not be published! User {0} with role {1} has no right to publish general description.'
  },
  messageGeneralDescriptionRoleActionWithdraw: {
    id: 'GeneralDescription.RoleException.MessageWithdraw',
    defaultMessage: 'General description can not be withdrawed! User {0} with role {1} has no right to withdraw general description.'
  },
  messageGeneralDescriptionRoleActionRestore: {
    id: 'GeneralDescription.RoleException.MessageRestore',
    defaultMessage: 'General description can not be restored! User {0} with role {1} has no right to restore general description.'
  },
  messageGeneralDescriptionRoleActionDelete: {
    id: 'GeneralDescription.RoleException.MessageDelete',
    defaultMessage: 'General description can not be deleted! User {0} with role {1} has no right to delete general description.'
  },
  messageGeneralDescriptionRoleActionLock: {
    id: 'GeneralDescription.RoleException.MessageLock',
    defaultMessage: 'General description can not be locked! User {0} with role {1} has no right to lock general description.'
  },
  messageGeneralDescriptionAdd: {
    id: 'GeneralDescription.AddGeneralDescription.MessageSaved',
    defaultMessage: 'Onnittelut! General description successfully saved.'
  },
  messageGeneralDescriptionSaved: {
    id: 'GeneralDescription.UpdateGeneralDescription.MessageSaved',
    defaultMessage: 'Onnittelut! General description successfully saved.'
  },
  messageGeneralDescriptionLocked: {
    id: 'GeneralDescription.Exception.MessageLock',
    defaultMessage: 'General description is locked by {0}!'
  },
  messageGeneralDescriptionPublished: {
    id: 'GeneralDescription.Published.Successfully',
    defaultMessage: 'Onnittelut! General description published.'
  },
  messageGeneralDescriptionWithdrawed: {
    id: 'GeneralDescription.Withdraw.Successfully',
    defaultMessage: 'Onnittelut! General description withdrawed.'
  },
  messageOrganizationCannotPublishDeletedRoot: {
    id: 'Organization.Exception.CannotPublishDeletedRoot',
    defaultMessage: 'Organization cannot be published because it is assigned to non published parent organization.'
  },
  messageGeneralDescriptionRestored: {
    id: 'GeneralDescription.Restore.Successfully',
    defaultMessage: 'Onnittelut! General description restored.'
  },
  messageGeneralDescriptionDeleted: {
    id: 'GeneralDescription.Deleted.Successfully',
    defaultMessage: 'Onnittelut! General description deleted.'
  },
  messageOrganizationMaxHierarchyLevel:{
    id:'Organization.Exception.MaxHierarchyLevel',
    defaultMessage: 'You already have max levels or organizations. You cannot create more that {0}-levels!'
  },
  messageTranslationOrderUpdateForbidden:{
    id:'Translation.TranslationException.MessageUpdateForbidden',
    defaultMessage: 'Cannot re-send translation order. Translation order must be in state delivered or delivered is confirm!'
  },
  messagePostponeTasksTitle:{
    id:'Tasks.Postpone.Message.Title',
    defaultMessage: 'Tehtävät lykätty.'
  },
  messageDraftServicesPostponed:{
    id:'Tasks.Draft.Services.Postponed.Message',
    defaultMessage: 'Tehtävät, jotka vanhentuvat alle 2 kk viikkoa, jäävät aktiivisina listalle.'
  },
  messagePublishedServicesPostponed:{
    id:'Tasks.Published.Services.Postponed.Message',
    defaultMessage: 'Tehtävät, jotka vanhentuvat alle 2 kk aikana, jäävät aktiivisina listalle.'
  },
  messageDraftChannelsPostponed:{
    id:'Tasks.Draft.Channels.Postponed.Message',
    defaultMessage: 'Tehtävät, jotka vanhentuvat alle 2 kk viikkoa, jäävät aktiivisina listalle.'
  },
  messagePublishedChannelsPostponed:{
    id:'Tasks.Published.Channels.Postponed.Message',
    defaultMessage: 'Tehtävät, jotka vanhentuvat alle 2 kk aikana, jäävät aktiivisina listalle.'
  },
  authorizationFailedTitle: {
    id: 'Authorization.Exception.LoginFailed',
    defaultMessage: 'Login failed'
  },
  authorizationFailedWrongCredentials: {
    id: 'Authorization.Exception.Type.WrongCredentials',
    defaultMessage: 'User or password are incorrect.'
  },
  authorizationFailedMissingOrganization: {
    id: 'Authorization.Exception.Type.MissingOrganization',
    defaultMessage: 'User has no organization assigned.'
  },
  authorizationFailedUserNotMapped: {
    id: 'Authorization.Exception.Type.UserNotMapped',
    defaultMessage: 'User has no role assigned.'
  },
  authorizationFailedNoOrgOrGroup: {
    id: 'Authorization.Exception.Type.NoOrgOrGroup',
    defaultMessage: 'User has no organization or role assigned.'
  },
  messageChannelSaveAddressTypeMismatchException: {
    id: 'Channel.Save.AddressTypeMismatch.MessageFailed',
    defaultMessage: 'When address type has value "Foreign" no other types of Visitng addresses can be defined.'
  },
  messagesAddressInvalidInfo: {
    id: 'Common.AddressException.InvalidInfo',
    defaultMessage: 'All address information must be filled in.'
  },
  messageGeneralDescriptionSaveSoteTypeException: {
    id: 'GeneralDescription.Save.GeneralDescriptionType.MessageFailed',
    defaultMessage: 'It is not possible to change \'Type of use area\' when SOTE general description is already in usage.'
  },
  messageSchedulePublishError: {
    id: 'Common.EntityException.SchedulePublishError',
    defaultMessage: 'It is not possible to schedule publishing of entity. Mandatory fields are missing.'
  },
  messageServiceScheduleDateError: {
    id: 'Common.ScheduleException.LateDate',
    defaultMessage: 'Publishing date cannot be scheduled after archiving date.'
  },
  massToolPublishSuccess: {
    id: 'MassTool.Publish.Success',
    defaultMessage: 'Published entities count {0}, published language versions count {1}, unpublished entities count {2}.'
  },
  massToolPublishValidationFailed: {
    id: 'MassTool.Exception.PublishValidationFailed',
    defaultMessage: 'Publish validation failed.'
  },
  massToolPublishValidationEntitiesFailed: {
    id: 'MassTool.Exception.PublishValidationEntitiesFailed',
    defaultMessage: 'Publish validation entities failed.'
  },
  massToolArchiveValidationEntitiesFailed: {
    id: 'MassTool.Exception.ArchiveValidationEntitiesFailed',
    defaultMessage: 'Archive validation entities failed.'
  },
  massToolCopyValidationEntitiesFailed: {
    id: 'MassTool.Exception.CopyValidationEntitiesFailed',
    defaultMessage: 'Copy validation entities failed.'
  },
  massToolMaxCountEntitiesValidationFailed: {
    id: 'MassTool.Exception.MaxCountLanguageVersionsValidationFailed',
    defaultMessage: 'Validation failed. Operation is available for maximal {0} language versions.'
  },
  massToolMaxCountLanguageVersionsValidationFailed: {
    id: 'MassTool.Exception.MaxCountEntitiesValidationFailed',
    defaultMessage: 'Validation failed. Operation is available for maximal {0} content type items.'
  },
  massToolMandatoryOrganizationValidationFailed: {
    id: 'MassTool.Exception.MandatoryOrganizationValidationFailed',
    defaultMessage: 'Validation failed. Target copy organization is missing.'
  },
  massToolArchiveSuccess: {
    id: 'MassTool.Archive.Success',
    defaultMessage: 'Entities archived successfully.'
  },
  massToolCopySuccess: {
    id: 'MassTool.Copy.Success',
    defaultMessage: 'Entities copied successfully.'
  },
  massToolPublishStarted: {
    id: 'MassTool.Publish.Started',
    defaultMessage: 'Mass publishing in progress, you may continue other functions.'
  },
  massToolArchiveStarted: {
    id: 'MassTool.Archive.Started',
    defaultMessage: 'Mass archiving in progress, you may continue other functions.'
  },
  massToolCopyStarted: {
    id: 'MassTool.Copy.Started',
    defaultMessage: 'Mass copying in progress, you may continue other functions.'
  }
})

export default Object.keys(messages).reduce(
  (prev, key) => {
    const curr = messages[key]
    prev[curr.id] = curr
    return prev
  },
  {}
)
