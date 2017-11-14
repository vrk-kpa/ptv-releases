import React from 'react'
import { Email } from 'util/redux-form/fields'
import {
  asCollection,
  asContainer,
  asLocalizableSection,
  asSection
} from 'util/redux-form/HOC'
import { compose } from 'redux'
import { injectIntl, FormattedMessage, defineMessages } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.EmailCollection.AddButton.Title',
    defaultMessage: '+ Uusi sähköpostiosoite'
  }
})

const EmailCollection = compose(
  injectIntl,
  asContainer({ title: messages.title, withCollection: true }),
  asLocalizableSection('emails'),
  asCollection({
    name: 'emails',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  }),
  asSection()
)(props => <Email {...props} />)

export default EmailCollection
