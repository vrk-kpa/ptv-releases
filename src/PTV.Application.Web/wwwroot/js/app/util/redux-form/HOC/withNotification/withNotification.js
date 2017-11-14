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
import React, { PropTypes } from 'react'
import { injectIntl } from 'react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Notification } from 'sema-ui-components'
import ImmutablePropTypes from 'react-immutable-proptypes'
import {
  injectFormName
} from 'util/redux-form/HOC'
import { getAllFormNotifications } from './selectors'
import messages from './messages'
import { hideMessage, keepOpen } from 'reducers/notifications'
import styles from './styles.scss'
import { Sticky, StickyContainer } from 'react-sticky'

// const messages = {}
const withNotification = ComposedComponent => {
  const Messages = ({
    notifications,
    hideMessage,
    keepOpen,
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
        id={id}
        title={getTitle(notification, id)}
        description={notification.get('stackTrace')}
        onRequestClose={onClose}
        type={notification.get('type') === 'error' ? 'fail' : 'ok'}
      />
    )

    return (
      <StickyContainer>
        <div className={styles.wrap}>
          <Sticky>
            <div className={styles.notification}>
              {notifications.map((n, index) => renderNotification(n, `${rest.formName}.${index}`)).toArray()}
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
    keepOpen: PropTypes.func.isRequired
  }

  return compose(
    injectFormName,
    injectIntl,
    connect(
      (state, { formName }) => ({
        notifications: getAllFormNotifications(state, { formName })
      }), {
        hideMessage,
        keepOpen
      }
    )
  )(Messages)
}
export default withNotification
