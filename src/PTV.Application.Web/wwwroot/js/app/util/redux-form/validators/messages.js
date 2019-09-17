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

export default defineMessages({
  draftJSbulletMaxCount: {
    id: 'Components.Validators.DraftJSMaxBulletCount.Message',
    defaultMessage: 'Text cannot start with list or contain bullet, number, letter list which length is over {limit}.', // eslint-disable-line
  },
  draftJSminLength: {
    id: 'Components.Validators.DraftJSMinLength.Message',
    defaultMessage: 'The text is too short.'
  },
  draftJSValueWithGDCheck: {
    id: 'Components.Validators.ValueWithGDCheck.Message',
    defaultMessage: 'The field is empty. Fill in the field to enable easy use of the service.'
  },
  invalidPublishingStatus: {
    id: 'Components.Validators.PublishingStatusRequired.Message',
    defaultMessage: 'Published status is required'
  },
  invalidPublishingOrganizationLanguageStatus:{
    id: 'Util.ReduxForm.Fields.ServiceOrganization.MissingTranslationWarning',
    defaultMessage: 'Organisation language version missing! Organisation must be described in the same language as the service.'
  },
  isEqual: {
    id: 'Components.Validators.IsEqual.Message',
    defaultMessage: 'Service name cannot be direct copy of summary.'
  },
  isEqualAtChannel: {
    id: 'Components.Validators.Channels.IsEqual.Message',
    defaultMessage: 'Channel name cannot be direct copy of summary.'
  },
  isRequired: {
    id: 'Components.Validators.IsRequired.Message',
    defaultMessage: 'Täytä kentän tiedot.'
  },
  itemMaxCountReached: {
    id: 'Components.Validators.ItemMaxCount.Message',
    defaultMessage: 'Limit of {limit} items reached.'
  },
  itemMaxCountExceeded: {
    id: 'Components.Validators.ItemMaxCountExceeded.Message',
    defaultMessage: 'Too many items, allowed {limit} or less.'
  },
  withoutSubLevelServiceClass: {
    id: 'Components.Validators.WithoutSubLevelServiceClass.Message',
    defaultMessage: 'Provide at least one sub level service class.'
  }
})

export const customValidationMessages = defineMessages({
  serviceClassItemMaxCountReached: {
    id: 'ServiceClass.ItemMaxCount.Message',
    defaultMessage: 'Valitse enintään neljä palveluluokkaa.',
    description: {
      en: 'Don\'t enter more than four service classes.',
      sv: 'Ge tjänsten högst fyra servicegrupp.'
    }
  },
  ontologyTermItemMaxCountReached: {
    id: 'OntologyTerm.ItemMaxCount.Message',
    defaultMessage: 'Do not enter more than ten keywords',
    description: 'Components.Validators.ItemMaxCount.Message'
  }
})
