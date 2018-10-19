import React from 'react'
import { Email } from 'util/redux-form/fields'
import asContainer from 'util/redux-form/HOC/asContainer'
import asCollection from 'util/redux-form/HOC/asCollection'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asSection from 'util/redux-form/HOC/asSection'
import { compose } from 'redux'
import { injectIntl, FormattedMessage, defineMessages } from 'util/react-intl'
import EmailTitle from './EmailTitle'

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
  asContainer({
    title: messages.title,
    withCollection: true,
    dataPaths: 'emails'
  }),
  asLocalizableSection('emails'),
  asCollection({
    name: 'emails',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    stacked: true,
    dragAndDrop: true,
    Title: EmailTitle
  }),
  asSection()
)(props => <div className='collection-form-row'><Email {...props} /></div>)

export default EmailCollection
