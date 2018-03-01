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
import PropTypes from 'prop-types'
import { defineMessages, injectIntl } from 'react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import ModalDialog from 'appComponents/ModalDialog'
import { uiErrorDialogCustomReducer } from 'reducers/ui'
import { createSelectors } from './selectors'
import { errorDialogKey } from 'enums'

const messages = defineMessages({
  dialogTitle: {
    id: 'Components.ServerErrorDialog.Title',
    defaultMessage: 'Sorry, something went wrong'
  },
  dialogDescription: {
    id: 'Components.ServerErrorDialog.Description',
    defaultMessage: 'Please try closing and re-opening your browser window.'
  },
  unauthorizedTitle: {
    id: 'Components.ServerErrorDialog.Unauthorized.Title',
    defaultMessage: 'Access was forbidden'
  },
  unauthorizedDescription: {
    id: 'Components.ServerErrorDialog.Unauthorized.Description',
    defaultMessage: 'Operation was not authorized, please relogin or use another user.'
  },
  connectionFailedTitle: {
    id: 'Components.ServerErrorDialog.Connection.Title',
    defaultMessage: 'Connection failed'
  },
  connectionFailedDescription: {
    id: 'Components.ServerErrorDialog.Connection.Description',
    defaultMessage: 'Connection problem occured during request. Please try again or later.'
  }
})

const getMessages = errorCode => {
  switch (errorCode) {
    case 401:
    case 403:
      return {
        title: messages.unauthorizedTitle,
        description: messages.unauthorizedDescription
      }
    case 'TypeError':
      return {
        title: messages.connectionFailedTitle,
        description: messages.connectionFailedDescription
      }
    default:
      return {
        title: messages.dialogTitle,
        description: messages.dialogDescription
      }
  }
}

const getFormattedMessages = (errorCode, formatMessage) => {
  const { title, description } = getMessages(errorCode)
  return {
    title: formatMessage(title),
    description: formatMessage(description)
  }
}

const ServerErrorDialog = (
  { intl: { formatMessage },
  errorCode
}) => {
  return (
    <ModalDialog
      name={errorDialogKey}
      {...getFormattedMessages(errorCode, formatMessage)}
      customReducer={uiErrorDialogCustomReducer}
      contentLabel='Server error'
    />
  )
}

ServerErrorDialog.propTypes = {
  intl: PropTypes.object.isRequired,
  errorCode: PropTypes.number
}

const { getErrorCode, getErrorName } = createSelectors(errorDialogKey)

export default compose(
  connect(
    state => ({
      errorCode: getErrorCode(state) || getErrorName(state)
    })
  ),
  injectIntl
)(ServerErrorDialog)
