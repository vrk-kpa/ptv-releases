import React from 'react'
import { PhoneNumbers } from 'util/redux-form/sections'
import {
  asCollection,
  asContainer,
  asLocalizableSection
} from 'util/redux-form/HOC'
import { compose } from 'redux'
import { injectIntl, FormattedMessage, defineMessages } from 'react-intl'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.PhoneNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi puhelinumero'
  }
})

const PhoneNumberCollection = compose(
  injectIntl,
  asContainer({ title: CommonMessages.phoneNumbers, simple: true }),
  asLocalizableSection('phoneNumbers'),
  asCollection({
    name: 'phoneNumbers',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(props => {
  return <PhoneNumbers {...props} />
})

export default PhoneNumberCollection
