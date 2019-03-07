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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Link } from 'react-router-dom'
import ModalDialog from 'appComponents/ModalDialog'
import {
  ModalActions
} from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { createSelectors } from './selectors'
import { errorDialogKey, ptvCookieTokenName, pahaCookieTokenName } from 'enums'
import { deleteCookie } from 'Configuration/AppHelpers'
import { withRouter } from 'react-router'
import { deleteUserInfo } from 'actions/init'

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
  reloginLink: {
    id: 'Components.ServerErrorDialog.Unauthorized.ReloginLink',
    defaultMessage: 'Relogin'
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

const getReloginLink = ({ errorCode, formatMessage, relogin, location }) => (
  (errorCode === 401 || errorCode === 403) &&
  <Link
    to={{
      pathname: '/login',
      state: { from: location }
    }}
    onClick={relogin}
  >{formatMessage(messages.reloginLink)}
  </Link>
)

const getFormattedMessages = (errorCode, formatMessage) => {
  const { title, description } = getMessages(errorCode)
  return {
    title: formatMessage(title),
    description: formatMessage(description)
  }
}

const ServerErrorDialog = ({
  intl: { formatMessage },
  errorCode,
  mergeInUIState,
  deleteUserInfo,
  location
}) => {
  const relogin = () => {
    deleteCookie(ptvCookieTokenName)
    deleteCookie(pahaCookieTokenName)
    deleteUserInfo()
    mergeInUIState({
      key: errorDialogKey,
      value: {
        isOpen: false
      }
    })
  }
  return (
    <ModalDialog
      name={errorDialogKey}
      {...getFormattedMessages(errorCode, formatMessage)}
      contentLabel='Server error'
    >
      <ModalActions>
        {getReloginLink({ errorCode, formatMessage, relogin, location })}
      </ModalActions>
    </ModalDialog>
  )
}

ServerErrorDialog.propTypes = {
  intl: intlShape.isRequired,
  errorCode: PropTypes.number,
  mergeInUIState: PropTypes.func.isRequired,
  deleteUserInfo: PropTypes.func.isRequired,
  location: PropTypes.object
}

const { getErrorCode, getErrorName } = createSelectors(errorDialogKey)

export default compose(
  connect(
    state => ({
      errorCode: getErrorCode(state) || getErrorName(state)
    }), {
      mergeInUIState,
      deleteUserInfo
    }
  ),
  injectIntl,
  withRouter
)(ServerErrorDialog)
