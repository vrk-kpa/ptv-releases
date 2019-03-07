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
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getParameterFromProps, getFormValue } from 'selectors/base'
import { defineMessages } from 'util/react-intl'

const messages = defineMessages({
  deletedLabel: {
    id: 'FrontPage.PublishingStatus.Deleted.Title',
    defaultMessage: 'Arkistoitu'
  },
  draftLabel: {
    id: 'FrontPage.PublishingStatus.Draft.Title',
    defaultMessage: 'Luonnos'
  }
})

export const getOrganizationStatus = createSelector(
  EntitySelectors.publishingStatuses.getEntity,
  (status) => status && status.get('code')
)

export const getPublishValidationExist = createSelector(
  [getParameterFromProps('languageCode'), getFormValue('languagesAvailabilities')],
  (language, avaliabilities) => {
    if (avaliabilities) {
      const avaliability = avaliabilities.first(x => x.get('code') === language)
      if (avaliability && avaliability.get('validatedFields')) {
        return avaliability.get('validatedFields').some(x => x.get('errorType') === 'publishedOrganizationLanguageMandatoryField')
      }
    }
    return false
  }
)

export const createOrganizationNameWithStatus = (organizationKey, getLabel = org => org.label) => createSelector(
  getOrganizationStatus,
  getParameterFromProps(organizationKey),
  getParameterFromProps('intl'),
  (code, organization, intl) => {
    switch (code) {
      case 'Draft':
      case 'Modified':
        return `${getLabel(organization)} (${intl.formatMessage(messages.draftLabel)})`
      case 'Deleted':
        return `${getLabel(organization)} (${intl.formatMessage(messages.deletedLabel)})`
      default:
        return getLabel(organization) || null
    }
  }
)
