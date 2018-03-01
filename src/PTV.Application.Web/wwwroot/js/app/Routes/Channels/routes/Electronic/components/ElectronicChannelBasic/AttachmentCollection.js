import React from 'react'
import {
  Attachment
} from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asCollection,
  asContainer,
  asLocalizableSection
} from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Attachment.Title',
    defaultMessage: 'Liitteet ja lisätietolinkit'
  },
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.AttachmentCollection.AddButton.Title',
    defaultMessage: '+ Uusi linkki'
  }
})

const AttachmentCollection = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    withCollection: true,
    dataPaths: 'attachments'
  }),
  asLocalizableSection('attachments'),
  asCollection({
    name: 'attachment',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(props => <Attachment {...props} />)

export default AttachmentCollection
