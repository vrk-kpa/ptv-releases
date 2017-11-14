import React from 'react'
import {
  asContainer,
  asCollection
} from 'util/redux-form/HOC'
import { compose } from 'redux'
import { AddressSwitch } from 'util/redux-form/sections'
import { addressUseCasesEnum } from 'enums'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Cahnnels.PostalAddress.Title',
    defaultMessage: 'Postiosoite (eri kuin käyntiosoite)'
  },
  addBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.PostalAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi postiosoite'
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

const PostalAddressCollection = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    simple: true
  }),
  asCollection({
    name: 'postalAddress',
    pluralName: 'postalAddresses',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(
  ({
  intl: { formatMessage },
  ...props
  }) =>
    <AddressSwitch
      mapDisabled
      additionalInformationProps={{
        title: formatMessage(messages.additionalInformationTitle),
        placeholder: formatMessage(messages.additionalInformationPlaceholder)
      }}
      addressUseCase={addressUseCasesEnum.POSTAL}
      {...props}
  />
)

export default PostalAddressCollection
