import React from 'react'
import { PhoneNumbersWithType } from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asGroup,
  asCollection,
  asLocalizableSection
} from 'util/redux-form/HOC'
import CommonMessages from 'util/redux-form/messages'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  addBtnTitle: {
    id : 'Routes.Channels.Phone.PhoneNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi puhelinumero'
  }
})

const PhoneNumberCollection = compose(
  injectIntl,
  asGroup({ title: CommonMessages.phoneNumbers }),
  asLocalizableSection('phoneNumbers'),
  asCollection({
    name: 'phoneNumber',
    simple: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(props => <PhoneNumbersWithType {...props} />)

export default PhoneNumberCollection
