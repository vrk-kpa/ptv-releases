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
import { connect } from 'react-redux'
import { defineMessages, injectIntl } from 'util/react-intl'
import { ErrorIcon } from 'appComponents/Icons'
import Popup from 'appComponents/Popup'
import ItemList from 'appComponents/ItemList'
import { branch } from 'recompose'
import { getErrorMessagesForUnapproved, getErrorMessagesForApproved } from './selectors'

const messages = defineMessages({
  languageVersionApproveMissing: {
    id: 'AppComponents.MassPublishTable.Errors.LanguageVersionApproveMissing',
    defaultMessage: 'Cannot publish, because another language versions needs to be approved. ({languages})'
  },
  cannotPublishTitle: {
    id: 'AppComponents.MassPublishTable.Errors.Title',
    defaultMessage: 'Cannot publish, see following errors:'
  }
})

export const PreviouslyPublishedUnapprovedMessage = compose(
  injectIntl
)(props =>
  <span>{props.intl.formatMessage(messages.languageVersionApproveMissing)}</span>
)

const getCustomKey = item => item.get('message') && item.get('message').id

const ErrorInformation = ({
  shouldPublish,
  language,
  errors,
  intl: { formatMessage }
}) =>
  errors && errors.size &&
  <Popup
    trigger={<span>
      <ErrorIcon size={20} />
    </span>}
    content={<ItemList
      type='error'
      title={formatMessage(messages.cannotPublishTitle)}
      items={errors}
      getCustomKey={getCustomKey}
      // showForLanguage={language}
    />}
    maxWidth='mW380'
    hideOnScroll
  /> || null

const mapStates = (state, { id, languageId, meta: { type }, ...rest }) => ({
  errors: getErrorMessagesForUnapproved(state, { id, languageId, type })
})

export const ErrorsForUnapproved = compose(
  injectIntl,
  branch(({ shouldPublish }) => shouldPublish, connect(mapStates))
)(ErrorInformation)

export const ErrorsForApproved = compose(
  injectIntl,
  connect((state, { unificRootId, formName }) => ({
    errors: getErrorMessagesForApproved(state, { id: unificRootId, formName })
  }))
)(ErrorInformation)
