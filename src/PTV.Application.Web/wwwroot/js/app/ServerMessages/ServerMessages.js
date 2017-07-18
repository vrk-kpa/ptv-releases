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
import { defineMessages, FormattedMessage } from 'react-intl'
const messages = defineMessages({
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
  messageAddServiceAndChannel: {
    id: 'ServiceAndChannel.AddServiceAndChannel.MessageSaved',
    defaultMessage: 'Onnittelut! Julkaistu-tilassa olevien palvelujen ja asiointikanavien liitokset on julkaistu.'
  },
  messagePublishServiceAndChannel: {
    id: 'ServiceAndChannel.PublishServiceAndChannel.MessageSaved',
    defaultMessage: 'Onnittelut!'
  },
  messageServiceLocked: {
    id: 'Service.Exception.MessageLock',
    defaultMessage: 'Service is locked by {0}!'
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
    messageOrganizationCyclicDependency:{
        id:"Organization.Exception.CyclicDependency",
        defaultMessage: "Assigned parent organization will cause cyclic dependency!"
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
  messageGeneralDescriptionRestored: {
    id: 'GeneralDescription.Restore.Successfully',
    defaultMessage: 'Onnittelut! General description restored.'
  },
  messageGeneralDescriptionDeleted: {
    id: 'GeneralDescription.Deleted.Successfully',
    defaultMessage: 'Onnittelut! General description deleted.'
  }
})

export function translateMessage (messageCode, formatMessage) {
  var message
  var args
  Object.keys(messages).forEach(function (key, index) {
    var messageArgs = messageCode.message ? messageCode.message.split(';') : messageCode.split(';')
    var mCode = messageArgs[0]
    messageArgs.shift()
    args = messageArgs
    if (messages[key].id == mCode) {
      message = messages[key]
    }
  })
  if (message) {
    return formatMessage(message, args)
  } else {
    return messageCode
  }
}

export function translateMessages (messageCodes, formatMessage) {
  var result = []
  messageCodes.forEach(function (code) {
    result.push(translateMessage(code, formatMessage))
  })
  return result
}

