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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { injectIntl } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import { Notification } from 'sema-ui-components'
import ImmutablePropTypes from 'react-immutable-proptypes'
// import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPortal from 'util/redux-form/HOC/withPortal'
import { getAllNotifications } from './selectors'
import messages from './messages'
import { hideMessage, keepOpen } from 'reducers/notifications'
import styles from './styles.scss'
import cx from 'classnames'
import { notificationTypesEnum, notificationButtonsEnum } from 'enums/notification'
import withState from 'util/withState'
import { Set } from 'immutable'

const getDescription = (notification, formatMessage) => {
  const params = notification.get('params')
  const code = notification.get('subCode')
  const stactTrace = notification.get('stackTrace')
  const message = code && messages[code] && formatMessage(messages[code], params && params || {}) || code
  const Component = notification.get('component')
  return Component
    ? <Component
      code={code}
      message={message}
      stactTrace={stactTrace}
      notification={notification}
      isDescription
      formatMessage={formatMessage}
    />
    : message || stactTrace
  // return (
  //   <div>
  //     <span>{message}</span>
  //     <span>{stactTrace}</span>
  //   </div>
  // )
}

const notificationType = {
  [notificationTypesEnum.info]: 'ok',
  [notificationTypesEnum.warning]: 'warn',
  [notificationTypesEnum.error]: 'fail'
}

const getId = (notification, id) => `${notification.get('key')}.${id}`

const getTitleMessage = (code, params, formatMessage) => {
  if (typeof code === 'object' && code.id && code.defaultMessage) {
    return formatMessage(code, params || {})
  } else if (messages[code]) {
    return formatMessage(messages[code], params || {})
  }
  return code
}

const showToggle = notification =>
  [notificationButtonsEnum.all, notificationButtonsEnum.toggle].includes(notification.get('buttons'))
const showClose = notification =>
  [notificationButtonsEnum.all, notificationButtonsEnum.close].includes(notification.get('buttons'))

const NotificationComponent = compose(
  branch(({ notifications }) => notifications.size, withPortal({ stickyOnScroll: true, stickyOffset: 80 }))
)(({ notifications, renderNotification }) => {
  return notifications.map((n, id) => renderNotification(n, id)).toArray()
})

const getTitle = (notification, fullId, formatMessage, getOnClick) => {
  const params = notification.get('params')
  const code = notification.get('code')
  const message = getTitleMessage(code, params, formatMessage)
  const Component = notification.get('component')
  return <div onClick={getOnClick(fullId)}>
    {Component
      ? <Component
        code={code}
        message={message}
        notification={notification}
        isTitle
        formatMessage={formatMessage}
      />
      : message
    }
  </div>
}

const withNotification = () => ComposedComponent => {
  const Messages = ({
    notifications,
    hideMessage,
    keepOpen,
    compact,
    updateUI,
    ...rest
  }) => {
    const onClose = id => hideMessage({ id })
    const onToggle = id => {
      const newCompact = compact.has(id) ? compact.delete(id) : compact.add(id)
      updateUI('compact', newCompact)
    }
    const onKeepOpenClick = id => () => keepOpen({ id })

    const renderNotification = (notification, id) => {
      const fullId = getId(notification, id)
      const isCompact = showToggle(notification) && compact.has(fullId)
      const notificationClass = cx(
        styles.notificationContent,
        {
          [styles.isCompact]: isCompact
        }
      )
      return (<Notification
        key={fullId}
        id={fullId}
        title={getTitle(notification, fullId, rest.intl.formatMessage, onKeepOpenClick)}
        description={getDescription(notification, rest.intl.formatMessage)}
        compact={isCompact}
        onRequestToggle={showToggle(notification) ? onToggle : undefined}
        onRequestClose={showClose(notification) ? onClose : undefined}
        type={notificationType[notification.get('type')] || notificationType.info}
        className={notificationClass}
      />
      )
    }

    return (
      <Fragment>
        <NotificationComponent
          notifications={notifications}
          renderNotification={renderNotification}
          componentClass={styles.notification}
        />
        <ComposedComponent {...rest} />
      </Fragment>
    )
  }

  Messages.propTypes = {
    notifications: ImmutablePropTypes.map.isRequired,
    hideMessage: PropTypes.func.isRequired,
    keepOpen: PropTypes.func.isRequired,
    compact: ImmutablePropTypes.set.isRequired,
    updateUI: PropTypes.func.isRequired
  }

  return compose(
    // branch(() => !formName, injectFormName),
    injectIntl,
    withState({
      initialState: {
        compact: Set()
      },
      keepImmutable: true
    }),
    connect(
      (state, ownProps) => ({
        notifications: getAllNotifications(state)
      }), {
        hideMessage,
        keepOpen
      }
    )
  )(Messages)
}
export default withNotification
