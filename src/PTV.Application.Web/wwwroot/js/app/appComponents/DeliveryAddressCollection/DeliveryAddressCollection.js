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
