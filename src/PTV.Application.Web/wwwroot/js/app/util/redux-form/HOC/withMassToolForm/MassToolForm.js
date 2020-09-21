/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { compose } from 'redux'
import { defineMessages } from 'util/react-intl'
import { reduxForm, change } from 'redux-form/immutable'
import { formTypesEnum, massToolTypes, timingTypes, notificationEnums } from 'enums'
import withSignalRHub from 'util/redux-form/HOC/withSignalRHub'
import { API_ROOT, getAuthToken } from 'Configuration/AppHelpers'
import {
  callMassPublish,
  callMassCopy,
  callMassArchive,
  callMassRestore,
  cancelReview,
  searchLatestPublished,
  searchCoppied,
  searchRestored,
  searchTaskRestored,
  refreshArchivedTasks
} from './actions'
import { getShowReviewBar } from './selectors'
import { Map } from 'immutable'
import ReviewBar from './ReviewBar'
import withMassDialog from 'util/redux-form/HOC/withMassDialog'
import { branch } from 'recompose'
import { mergeInUIState } from 'reducers/ui'
import { getShowWarningAction, getShowInfosAction, hideMessage } from 'reducers/notifications'
import { withRouter } from 'react-router'
import { Button } from 'sema-ui-components'
import { connect } from 'react-redux'
import withState from 'util/withState'
import { clearSelection } from 'Routes/Search/components/MassTool/actions'

const messages = defineMessages({
  massPublishSuccessLink: {
    id: 'MassTool.Publish.Success.Link',
    defaultMessage: 'Näytä julkaistut sisällöt'
  },
  massCopySuccessLink: {
    id: 'MassTool.Copy.Success.Link',
    defaultMessage: 'Load copied content'
  },
  massRestoreSuccessLink: {
    id: 'MassTool.Restore.Success.Link',
    defaultMessage: 'Julkaise sisällöt'
  }
})

const MassToolForm = ({ isReviewVisible }) => {
  return (
    isReviewVisible && <ReviewBar />
  )
}

MassToolForm.propTypes = {
  isReviewVisible: PropTypes.bool
}

const hubName = 'massTool'

const progresInfoAction = getShowWarningAction(
  hubName,
  false,
  notificationEnums.notificationButtonsEnum.toggle
)

const Link = compose(
  connect(null, { onClick: searchLatestPublished })
)(Button)

const CopyLink = compose(
  connect(null, { onClick: searchCoppied })
)(Button)

const RestoreLink = compose(
  connect(null, { onClick: searchRestored })
)(Button)

const RestoreTaskLink = compose(
  connect(null, { onClick: searchTaskRestored })
)(Button)

const LinkComponent = compose(
  withRouter
)(({
  notification,
  isDescription,
  message,
  formatMessage,
  location
}) => {
  return (isDescription &&
    notification.get('code') === 'MassTool.Publish.Success' &&
    location.pathname === '/frontpage/search' && (<Link link>{formatMessage(messages.massPublishSuccessLink)}</Link>) ||
    isDescription &&
    notification.get('code') === 'MassTool.Copy.Success' &&
    location.pathname === '/frontpage/search' && (<CopyLink link>{formatMessage(messages.massCopySuccessLink)}</CopyLink>) ||
    isDescription &&
    notification.get('code') === 'MassTool.Restore.Success' &&
    location.pathname === '/frontpage/search' && (<RestoreLink link>{formatMessage(messages.massRestoreSuccessLink)}</RestoreLink>) ||
    isDescription &&
    notification.get('code') === 'MassTool.Restore.Success' &&
    location.pathname === '/frontpage/tasks' && (<RestoreTaskLink link>{formatMessage(messages.massRestoreSuccessLink)}</RestoreTaskLink>) ||
    message)
})

const publishDoneInfoAction = getShowInfosAction(
  hubName,
  false,
  notificationEnums.notificationButtonsEnum.close,
  notification => {
    notification = notification.set('component', LinkComponent)
    if (notification.get('code') === 'MassTool.Restore.Success') {
      notification = notification.set('type', 'warning')
    }
    return notification
  }
)

const processStarted = (message, { dispatch }) => {
  dispatch(mergeInUIState({
    key: 'MassToolDialog',
    value: {
      isOpen: false
    }
  }))
  dispatch(cancelReview())
  dispatch(clearSelection())
}

const processStartedNotification = ({ dispatch }) => (message) => {
  if (message) {
    dispatch(progresInfoAction([ message ]))
  }
}

const processFinished = ({ dispatch }) => (message) => {
  if (message && message.id) {
    dispatch(hideMessage({ id: [ hubName, message.id ] }))
  }
  dispatch(refreshArchivedTasks())
}

const onSubmit = (values, dispatch, props) => {
  switch (values.get('type')) {
    case massToolTypes.PUBLISH:
      return dispatch(callMassPublish(values, props.hubInvoke))
    case massToolTypes.ARCHIVE:
      return dispatch(callMassArchive(values, props.hubInvoke))
    case massToolTypes.RESTORE:
      return dispatch(callMassRestore(values, props.hubInvoke))
    case massToolTypes.COPY:
      return dispatch(callMassCopy(values, props.hubInvoke))
    default:
      return console.log('Type not handled')
  }
}

const onSubmitSuccess = (result, dispatch, props) => {
  // handleOnSubmitSuccess(result, dispatch, props, getService)
}

const onSubmitFail = (...args) => console.log(args)

export default compose(
  branch(() => getAuthToken(), withSignalRHub({ hubName,
    showNotification: true,
    messageReceived: processStarted,
    hubLink: new URL(API_ROOT).origin + '/massToolHub',
    actionDefinition: [{
      type: 'ProcessStarted',
      action: processStartedNotification
    }, {
      type: 'ProcessFinished',
      action: processFinished
    }],
    onInfoAction: publishDoneInfoAction
  })),
  reduxForm({
    form: formTypesEnum.MASSTOOLFORM,
    initialValues: {
      timingType: timingTypes.NOW,
      type: massToolTypes.PUBLISH
    },
    destroyOnUnmount: false,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess
  }),
  withMassDialog,
  connect(
    state => ({
      isReviewVisible: getShowReviewBar(state)
    })
  ),
  withState({
    redux: true,
    key: 'entityReview',
    initialState: {
      reviewed: false
    }
  })
)(MassToolForm)
