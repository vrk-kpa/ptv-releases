/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
  isEqualAtOrganization: {
    id: 'Components.Validators.Organization.IsEqual.Message',
    defaultMessage: 'Et voi julkaista kuvausta, jos tiivistelmä on kopio organisaation nimestä.',
    description: {
      sv: 'Du kan inte publicera beskrivningen om referatet är en kopia på organisationens namn.',
      en: 'You cannot publish a description if the summary is a copy of the organisation’s name.'
    }
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
  },
  isDuplicate: {
    id: 'Components.Validators.isDuplicate.Message',
    defaultMessage: 'You cannot publish a name, if same name already exists in another content within your organization.'
  },
  isUrl: {
    id: 'Components.Validators.IsUrl.Message',
    defaultMessage: 'Täytä tarkka verkko-osoite.'
  },
  isInvalidUrl: {
    id: 'Components.Validators.isInvalidUrl.Message',
    defaultMessage: 'The provided URL could not have been validated.'
  },
  minLengthLimit: {
    id: 'Components.Validators.minLengthLimit.Message',
    defaultMessage: 'At least five characters are required for this field.'
  },
  connectionTypeAsti: {
    id: 'Components.Validators.connectionTypeAsti.Message',
    defaultMessage: 'Kanavien yhteiskäyttöisyyttä ei voi muuttaa, sillä palvelupaikka on ASTI-palvelupaikka.'
  }
})

export const customValidationMessages = defineMessages({
  ontologyTermItemMaxCountReached: {
    id: 'OntologyTerm.ItemMaxCount.Message',
    defaultMessage: 'Do not enter more than ten keywords',
    description: 'Components.Validators.ItemMaxCount.Message'
  }
})
