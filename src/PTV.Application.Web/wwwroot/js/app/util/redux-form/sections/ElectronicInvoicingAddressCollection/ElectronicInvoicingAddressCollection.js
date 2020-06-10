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
import React from 'react'
import { OrganizationElectronicInvoicingAddress } from 'util/redux-form/sections'
import { compose } from 'redux'
import { withProps } from 'recompose'
import asContainer from 'util/redux-form/HOC/asContainer'
import asCollection from 'util/redux-form/HOC/asCollection'
import asSection from 'util/redux-form/HOC/asSection'
import ElectronicInvoicingAddressTitle from './ElectronicInvoicingAddressTitle'
import commonMessages from 'util/redux-form/messages'
import { defineMessages, injectIntl, FormattedMessage } from 'util/react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Manage.Organizations.Manage.ElectronicInvoicingAddressCollection.Title',
    defaultMessage: 'Verkkolaskutusosoite'
  },
  additionalInformationTitle: {
    id: 'Containers.Organizations.ElectronicInvoicingAddress.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  additionalInformationPlaceholder: {
    id: 'Containers.Organizations.ElectronicInvoicingAddress.AdditionalInformation.Placeholder',
    defaultMessage: 'Kirjoita esim. operaattorin nimi tai muu tarvittava lisätieto'
  },
  addBtnTitle: {
    id: 'Routes.Organization.ElectronicInvoicingAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi verkkolaskutusosoite'
  }
})

const ElectronicInvoicingAddressCollection = compose(
  injectIntl,
  asContainer({
    title: commonMessages.electronicInvoicingAddresses,
    dataPaths: 'electronicInvoicingAddresses'
  }),
  withProps(props => ({
    comparable: true
  })),
  asCollection({
    name: 'electronicInvoicingAddress',
    pluralName: 'electronicInvoicingAddresses',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    stacked: true,
    dragAndDrop: true,
    Title: ElectronicInvoicingAddressTitle
  }),
  asSection()
)(({
  intl: { formatMessage },
  ...rest
}) => <OrganizationElectronicInvoicingAddress
  additionalInformationProps={{
    title: formatMessage(messages.additionalInformationTitle),
    placeholder: formatMessage(messages.additionalInformationPlaceholder)
  }}
  {...rest}
/>)

export default ElectronicInvoicingAddressCollection
