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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import NotificationDialog, { NotificationDialogOld } from './NotificationDialog'
import { Button } from 'sema-ui-components'
import { getWarningChannels, getNotificationIds } from '../selectors'
import { getContentLanguageCode } from 'selectors/selections'
import { withRouter } from 'react-router'
import { camelCase } from 'lodash'
import { getEntityInfo } from 'enums'
import withReturnLink from 'util/redux-form/HOC/withReturnLink'
import { removeNotifications } from '../actions'
import messages from '../messages'

const getName = (name, languageCode) => {
  if (name.toJS) {
    name = name.toJS()
  }
  const result = name && (
    name[languageCode] ||
    name[Object.keys(name)[0]]
  )
  return result
}

const navigateToChannel = (id, type, goBackWithReturnLink) => {
  const meta = getEntityInfo('channel', camelCase(type), false)
  const pathname = meta.path + '/' + id
  goBackWithReturnLink(pathname)
}

const AffectedChannelLinks = ({
  channels,
  languageCode,
  goBackWithReturnLink
}) => {
  return (
    <ul>
      {channels.map((item, index) => (
        <li key={index}>
          <Button
            type='button'
            link
            onClick={() =>
              navigateToChannel(
                item.get('channelVersionedId'),
                item.get('channelType'),
                pathname => goBackWithReturnLink(pathname))}
          >
            {getName(item.get('name'), languageCode)}
          </Button>
        </li>
      ))}
    </ul>
  )
}

AffectedChannelLinks.propTypes = {
  channels: ImmutablePropTypes.list,
  languageCode: PropTypes.string,
  goBackWithReturnLink: PropTypes.func
}

const RemoveConnectionWarningDialog = compose(
  injectIntl,
  withRouter,
  withReturnLink,
  connect(state => ({
    channels: getWarningChannels(state),
    languageCode: getContentLanguageCode(state),
    notificationIds: getNotificationIds(state)
  }))
)(({ intl: { formatMessage }, notificationIds, dispatch, ...rest }) => {
  const handleRemovingNotifications = (ids) => {
    dispatch(removeNotifications(ids))
  }
  return (
    <NotificationDialogOld
      text={formatMessage(messages.removeConnectionWarningTitle)}
      customAction={() => handleRemovingNotifications(notificationIds)}
    >
      <AffectedChannelLinks {...rest} />
    </NotificationDialogOld>
  )
})

RemoveConnectionWarningDialog.propTypes = {
  dispatch: PropTypes.func,
  notificationIds: PropTypes.any,
  intl: intlShape
}

export default RemoveConnectionWarningDialog
