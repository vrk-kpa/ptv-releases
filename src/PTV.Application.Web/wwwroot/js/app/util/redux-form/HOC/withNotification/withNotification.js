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
import { injectIntl } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Notification } from 'sema-ui-components'
import ImmutablePropTypes from 'react-immutable-proptypes'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getAllFormNotifications } from './selectors'
import messages from './messages'
import { hideMessage, keepOpen } from 'reducers/notifications'
import styles from './styles.scss'
import { Sticky, StickyContainer } from 'react-sticky'
import { branch } from 'recompose'

const getDescription = (notification, formatMessage) => {
  const params = notification.get('params')
  const code = notification.get('subCode')
  const stactTrace = notification.get('stackTrace')
  const message = code && messages[code] && formatMessage(messages[code], params && params.split(';') || {}) || code
  return message || stactTrace
  // return (
  //   <div>
  //     <span>{message}</span>
  //     <span>{stactTrace}</span>
  //   </div>
  // )
}

const withNotification = ({
  formName
} = {}) => ComposedComponent => {
  const Messages = ({
    notifications,
    hideMessage,
    keepOpen,
    notificationFormName,
    ...rest
  }) => {
    const onClose = id => hideMessage({ id })
    const onKeepOpenClick = id => () => keepOpen({ id })

    const getTitle = (notification, id) => {
      const params = notification.get('params')
      const code = notification.get('code')
      const message = messages[code] &&
        rest.intl.formatMessage(messages[code], params && params.split(';') || {}) ||
        code
      return <div onClick={onKeepOpenClick(id)}>{message}</div>
    }

    const renderNotification = (notification, id) => (
      <Notification
        key={id}
        id={id}
        title={getTitle(notification, id)}
        description={getDescription(notification, rest.intl.formatMessage)}
        onRequestClose={onClose}
        type={notification.get('type') === 'error' ? 'fail' : 'ok'}
      />
    )

    return (
      <StickyContainer>
        <div className={styles.wrap}>
          <Sticky>
            <div className={styles.notification}>
              {notifications.map((n, index) => renderNotification(n, `${notificationFormName}.${index}`)).toArray()}
            </div>
          </Sticky>
          <ComposedComponent {...rest} />
        </div>
      </StickyContainer>
    )
  }

  Messages.propTypes = {
    notifications: ImmutablePropTypes.map,
    hideMessage: PropTypes.func.isRequired,
    keepOpen: PropTypes.func.isRequired,
    notificationFormName: PropTypes.string.isRequired
  }

  return compose(
    branch(() => !formName, injectFormName),
    injectIntl,
    connect(
      (state, ownProps) => ({
        notifications: getAllFormNotifications(state, { formName: formName || ownProps.formName }),
        notificationFormName: formName || ownProps.formName
      }), {
        hideMessage,
        keepOpen
      }
    )
  )(Messages)
}
export default withNotification
