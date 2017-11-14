import React from 'react'
import { AddressSwitch } from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asGroup,
  asCollection
} from 'util/redux-form/HOC'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import { addressUseCasesEnum } from 'enums'
import CommonMessages from 'util/redux-form/messages'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  addBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.VisitingAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi käyntiosoite'
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

const VisitingAddressCollection = compose(
  asGroup({ title: CommonMessages.visitingAddresses }),
  // asLocalizableSection('visitingAddresses'),
  withErrorDisplay('visitingAddresses'),
  asCollection({
    name: 'visitingAddress',
    pluralName: 'visitingAddresses',
    simple: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(({
  intl: { formatMessage },
  ...rest
}) =>
  <AddressSwitch
    addressUseCase={addressUseCasesEnum.VISITING}
    additionalInformationProps={{
      title: formatMessage(messages.additionalInformationTitle),
      placeholder: formatMessage(messages.additionalInformationPlaceholder)
    }}
    {...rest}
  />
)

export default VisitingAddressCollection
