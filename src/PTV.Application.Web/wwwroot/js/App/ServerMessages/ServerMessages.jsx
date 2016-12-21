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
import { defineMessages, FormattedMessage } from 'react-intl';
const messages = defineMessages({
    messageServiceSaved: {
        id: "Service.AddService.MessageSaved",
        defaultMessage: "Onnittelut! Palvelun tallennus onnistui."
    },
    messageOrganizationSaved: {
        id: "Organization.AddOrganization.MessageSaved",
        defaultMessage: "Onnittelut! Organisaatio tallennus onnistui."
    },
    messageServicePublished: {
        id: "Service.EditService.MessagePublished",
        defaultMessage: "Onnittelut! Palvelun julkaisu onnistui."
    },
    messageOrganizationPublished: {
        id: "Organization.EditOrganization.MessagePublished",
        defaultMessage: "Onnittelut! Organisaatio julkaisu onnistui."
    },
    messageServiceDeleted: {
        id: "Service.EditService.MessageDeleted",
        defaultMessage: "Onnittelut! Palvelu on poistettu."
    },
    messageOrganizationDeleted: {
        id: "Organization.EditOrganization.MessageDeleted",
        defaultMessage: "Onnittelut! Organisaatio on poistettu."
    },
    messageElectronicChannelSaved: {
        id: "Service.AddElectronicChannel.MessageSaved",
        defaultMessage: "Onnittelut! Asiointikanavan tallennus onnistui."
    }, 
    messageChannelPublished: {
        id: "Channel.Published.Successfully",
        defaultMessage: "Onnittelut! Asiointikanavan julkaisu onnistui."
    },
    messageChannelDeleted: {
        id: "Channel.Deleted.Successfully",
        defaultMessage: "Onnittelut! Asiointikanava on poistettu"
    }, 
    messagePhoneChannelSaved: {
        id: "Service.AddPhoneChannel.MessageSaved",
        defaultMessage: "Onnittelut! Asiointikanavan tallennus onnistui."
    },
    messageLocationChannelSaved: {
        id: "Service.AddLocationChannel.MessageSaved",
        defaultMessage: "Onnittelut! Asiointikanavan tallennus onnistui."
    },
    messageWebPageChannelSaved: {
        id: "Service.AddWebPageChannel.MessageSaved",
        defaultMessage: "Onnittelut! Asiointikanavan tallennus onnistui."
    },
    messageServiceStepSaved: {
        id: "Service.EditService.MessageStepSaved",
        defaultMessage: "Onnittelut! Tiedot tallennettu."
    },
    messageOrganizationStepSaved: {
        id: "Organization.OrganizationStep.MessageSaved",
        defaultMessage: "Onnittelut! Tiedot tallennettu."
    },
    messageChannelStepSaved: {
        id: "Service.ChannelStep.MessageSaved",
        defaultMessage: "Onnittelut! Tiedot tallennettu."
    },

    messageInvalidArgument: {
        id: "Service.Exception.InvalidArgument",
        defaultMessage: "Invalid arguments : {0}"
    },
    messageDuplicityArgument: {
        id: "Organization.Exception.DuplicityCheck",
        defaultMessage: "OID monistaa : {0}!"
    },
    messageServiceNotFound: {
        id: "Service.Exception.NotFound",
        defaultMessage: "Service with ID {0} not found!"
    },
    messageChannelNotFound: {
        id: "ServiceChannel.Exception.NotFound",
        defaultMessage: "Channel with ID {0} not found!"
    },
    messageOrganizationNotFound: {
        id: "Organization.Exception.NotFound",
        defaultMessage: "Organization with ID {0} not found!"
    },
    messageAddServiceAndChannel:{
        id: "ServiceAndChannel.AddServiceAndChannel.MessageSaved",
        defaultMessage: "Onnittelut! Julkaistu-tilassa olevien palvelujen ja asiointikanavien liitokset on julkaistu."
    },
    messagePublishServiceAndChannel:{
        id: "ServiceAndChannel.PublishServiceAndChannel.MessageSaved",
        defaultMessage: "Onnittelut!"
    },    
});

export function translateMessage(messageCode, formatMessage){
   var message;
   var args;
   Object.keys(messages).forEach(function(key,index) {
        var messageArgs = messageCode.message ? messageCode.message.split(';') : messageCode.split(';');
        var mCode = messageArgs[0];
        messageArgs.shift();
        args = messageArgs;
        if(messages[key].id == mCode){
            message = messages[key]
        }
    });
    if(message){
        return formatMessage(message, args);
    }
    else{
        return messageCode;
    }
}

export function translateMessages(messageCodes, formatMessage){
   var result = [];
   messageCodes.forEach(function(code){
    		result.push(translateMessage(code, formatMessage))
    	})
   return result;
}

