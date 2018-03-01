import React from 'react'
import { compose } from 'redux'
import { Attachment } from 'util/redux-form/sections'
import {
  asContainer,
  asCollection,
  asLocalizableSection
} from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Title',
    defaultMessage: 'Liitteet ja lisätietolinkit'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Tooltip',
    defaultMessage: 'Voit antaa linkkejä verkkoasiointikanavaan liittyviin ohjedokumentteihin tai -sivuihin.'
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
    tooltip: messages.tooltip,
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
