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
import { AddressSwitch } from 'util/redux-form/sections'
import { compose } from 'redux'
import { withProps } from 'recompose'
import asGroup from 'util/redux-form/HOC/asGroup'
import asCollection from 'util/redux-form/HOC/asCollection'
import asSection from 'util/redux-form/HOC/asSection'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import { addressUseCasesEnum } from 'enums'
import CommonMessages from 'util/redux-form/messages'
import { defineMessages, FormattedMessage } from 'util/react-intl'
import DeliveryAddressTitle from './DeliveryAddressTitle'

const messages = defineMessages({
  addBtnTitle: {
    id : 'Routes.Channels.PrintableForm.DeliveryAddressCollection.AddButton.Title',
    defaultMessage: '+ New delivery address'
  },
  addNewBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.DeliveryAddressCollection.AddNewButton.Title',
    defaultMessage: 'Lisää osoite'
  },
  additionalInformationTitle: {
    id: 'Containers.Channels.Address.AdditionalInformation.Title',
    defaultMessage: 'Osoitteen lisätiedot'
  },
  additionalInformationPlaceholder: {
    id: 'Containers.Channels.Address.AdditionalInformation.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

const DeliveryAddressCollection = compose(
  withErrorDisplay('deliveryAddresses'),
  asGroup({ title: CommonMessages.deliveryAddresses }),
  withProps(props => ({
    comparable: true,
    defaultStreetTitle: true
  })),
  asCollection({
    name: 'deliveryAddress',
    pluralName: 'deliveryAddresses',
    simple: true,
    stacked: true,
    dragAndDrop: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    addNewBtnTitle: <FormattedMessage {...messages.addNewBtnTitle} />,
    Title: DeliveryAddressTitle
  }),
  asSection()
)(({
  intl: { formatMessage },
  ...rest
}) =>
  <AddressSwitch
    mapDisabled
    addressUseCase={addressUseCasesEnum.DELIVERY}
    additionalInformationProps={{
      title: formatMessage(messages.additionalInformationTitle),
      placeholder: formatMessage(messages.additionalInformationPlaceholder)
    }}
    {...rest}
  />
)

export default DeliveryAddressCollection
