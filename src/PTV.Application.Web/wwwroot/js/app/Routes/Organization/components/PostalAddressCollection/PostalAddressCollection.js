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
import React from 'react'
import { AddressSwitch } from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asCollection,
  asContainer
} from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'
import { addressUseCasesEnum } from 'enums'

const messages = defineMessages({
  title: {
    id: 'Containers.Manage.Organizations.Manage.Step1.showPostalAddress',
    defaultMessage: 'Lisää postiosoite'
  },
  additionalInformationTitle: {
    id: 'Containers.Channels.Address.AdditionalInformation.Title',
    defaultMessage: 'Osoitteen lisätiedot'
  },
  additionalInformationPlaceholder: {
    id: 'Containers.Channels.Address.AdditionalInformation.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  addBtnTitle: {
    id: 'Routes.Organization.PostalAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi postiosoite'
  }
})
const PostalAddressCollection = compose(
  injectIntl,
  asContainer({ title: messages.title, simple: true }),
  asCollection({
    pluralName: 'postalAddresses',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(({
  intl: { formatMessage },
  ...rest
}) => <AddressSwitch
  mapDisabled
  additionalInformationProps={{
    title: formatMessage(messages.additionalInformationTitle),
    placeholder: formatMessage(messages.additionalInformationPlaceholder)
  }}
  addressUseCase={addressUseCasesEnum.POSTAL}
  {...rest}
  />)
export default PostalAddressCollection
