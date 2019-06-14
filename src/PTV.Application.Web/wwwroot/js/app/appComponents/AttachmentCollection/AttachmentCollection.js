/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
import { compose } from 'redux'
import { Attachment } from 'util/redux-form/sections'
import asContainer from 'util/redux-form/HOC/asContainer'
import asCollection from 'util/redux-form/HOC/asCollection'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import { defineMessages, injectIntl, FormattedMessage } from 'util/react-intl'
import commonMessages from 'util/redux-form/messages'
import AttachmentTitle from './AttachmentTitle'

const messages = defineMessages({
  addBtnTitleAttachment: {
    id : 'Util.ReduxForm.Sections.AttachmentCollection.AddButton.Title',
    defaultMessage: '+ Uusi linkki'
  },
  addBtnTitleWebpage: {
    id : 'Routes.Channels.ServiceLocation.AttachmentCollection.AddButton.Title',
    defaultMessage: '+ Uusi verkkosivu'
  },
  addNewBtnTitleAttachment: {
    id : 'AppComponents.AttachmentCollection.Attachment.AddNewButton.Title',
    defaultMessage: 'Lisää linkki'
  },
  addNewBtnTitleWebpage: {
    id : 'AppComponents.AttachmentCollection.Webpage.AddNewButton.Title',
    defaultMessage: 'Lisää verkkosivu'
  }
})

const AttachmentCollectionContainer = compose(
  injectIntl,
  asContainer({
    title: commonMessages.attachments,
    withCollection: true,
    dataPaths: 'attachments'
  }),
  asLocalizableSection('attachments'),
  asCollection({
    name: 'attachment',
    stacked: true,
    dragAndDrop: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitleAttachment} />,
    addNewBtnTitle: <FormattedMessage {...messages.addNewBtnTitleAttachment} />,
    Title: AttachmentTitle
  })
)(Attachment)

const AttachmentCollectionNested = compose(
  injectIntl,
  asContainer({
    title: commonMessages.webPages,
    dataPaths: 'webPages'
  }),
  asLocalizableSection('webPages'),
  asCollection({
    name: 'webPage',
    stacked: true,
    dragAndDrop: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitleWebpage} />,
    addNewBtnTitle: <FormattedMessage {...messages.addNewBtnTitleWebpage} />,
    Title: AttachmentTitle
  })
)(props => <Attachment hideDescription {...props} />)

export const AttachmentCollectionECH = AttachmentCollectionContainer
export const AttachmentCollectionPRNT = AttachmentCollectionContainer
export const AttachmentCollectionSL = AttachmentCollectionNested
export const AttachmentCollectionORG = AttachmentCollectionNested
